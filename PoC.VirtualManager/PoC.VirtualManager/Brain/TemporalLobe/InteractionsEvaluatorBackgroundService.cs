using Azure.AI.TextAnalytics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.VisualBasic;
using PoC.VirtualManager.Brain.FrontalLobe;
using PoC.VirtualManager.Brain.FrontalLobe.Personality;
using PoC.VirtualManager.Brain.FrontalLobe.Personality.Leadership;
using PoC.VirtualManager.Brain.ParietalLobe.Extensions;
using PoC.VirtualManager.Interactions.Extensions;
using PoC.VirtualManager.Interactions.Infrastructure;
using PoC.VirtualManager.Interactions.Models;
using PoC.VirtualManager.Interactions.Models.Extensions;
using PoC.VirtualManager.Slack.Client.Models.Messaging;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;
using ChannelId = System.String;

namespace PoC.VirtualManager.Brain.TemporalLobe
{
    public class InteractionsEvaluatorBackgroundService : BackgroundService
    {
        private readonly ILogger<SlackProcessorBackgroundService> _logger;
        private readonly Channel<SlackInteractionsQueueItem> _slackInteractionsChannel;
        private readonly TextAnalyticsClient _textAnalyticsClient;
        private readonly IInteractionsMetadataRepository _interactionsRepository;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly Kernel _kernel;
        private readonly Personality _personality;

        private readonly ConcurrentDictionary<ChannelId, ChatHistory> _chatsHistory;

        public InteractionsEvaluatorBackgroundService(ILogger<SlackProcessorBackgroundService> logger, 
            [FromKeyedServices("slack-interactions-channel")] Channel<SlackInteractionsQueueItem> slackInteractionsChannel, 
            TextAnalyticsClient textAnalyticsClient,
            IInteractionsMetadataRepository interactionsRepository,
            IChatCompletionService chatCompletionService,
            Personality personality,
            Kernel kernel)
        {
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ArgumentNullException.ThrowIfNull(slackInteractionsChannel, nameof(slackInteractionsChannel));
            ArgumentNullException.ThrowIfNull(textAnalyticsClient, nameof(textAnalyticsClient));
            ArgumentNullException.ThrowIfNull(interactionsRepository, nameof(interactionsRepository));
            ArgumentNullException.ThrowIfNull(chatCompletionService, nameof(chatCompletionService));
            ArgumentNullException.ThrowIfNull(personality, nameof(personality));
            ArgumentNullException.ThrowIfNull(kernel, nameof(kernel));

            _logger = logger;
            _slackInteractionsChannel = slackInteractionsChannel;
            _textAnalyticsClient = textAnalyticsClient;
            _interactionsRepository = interactionsRepository;
            _chatCompletionService = chatCompletionService;
            _personality = personality;
            _kernel = kernel;
            _chatsHistory = new ConcurrentDictionary<ChannelId, ChatHistory>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var interaction in _slackInteractionsChannel.Reader.ReadAllAsync(stoppingToken))
            {
                _logger.LogInformation($"Consumed interaction");

                if(interaction == null || !interaction.Messages.Any())
                {
                    _logger.LogInformation($"Interaction is empty, wont be processed for metadata.");
                    continue;
                }

                var documentsChunks = interaction
                    .Messages
                    .Select(m => new TextDocumentInput(id: Guid.NewGuid().ToString(), text: m.ToString()))
                    .SplitIntoChunks(chunkSize: 10)
                    .ToList();

                foreach (var documents in documentsChunks)
                {
                    AnalyzeSentimentResultCollection reviews = 
                        await _textAnalyticsClient.AnalyzeSentimentBatchAsync(
                            documents, 
                            options: new AnalyzeSentimentOptions()
                            {
                                IncludeOpinionMining = true,
                            }, 
                            cancellationToken: stoppingToken);

                    await ProcessInteractionSentimentAsync(interaction, documents, reviews, stoppingToken);
                }
            }
        }

        private async Task ProcessInteractionSentimentAsync(
            SlackInteractionsQueueItem interactions,
            List<TextDocumentInput> documents, 
            AnalyzeSentimentResultCollection reviews,
            CancellationToken cancellationToken)
        {
            foreach (var review in reviews)
            {
                var document = documents.FirstOrDefault(d => d.Id == review.Id);
                var message = interactions.Messages.FirstOrDefault(m => document.Text.Contains(m.Message));

                _logger.LogInformation($"Document: \n{JsonSerializer.Serialize(document)}\nSentiment analysis: \n{JsonSerializer.Serialize(review)}");

                var chatHistory = GetOrCreateChatHistory(interactions);

                var promptWithInteractionAndReview =
                    $@"Interaction: {JsonSerializer.Serialize(message)}";

                chatHistory.AddUserMessage(promptWithInteractionAndReview);
                var interactionMetadataJson = (await _chatCompletionService.GetChatMessageContentAsync(
                                chatHistory,
                                executionSettings: new OpenAIPromptExecutionSettings()
                                {
                                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                                }, 
                                kernel: _kernel,
                                cancellationToken
                                )).Content;

                chatHistory.AddAssistantMessage(interactionMetadataJson);

                if (!interactionMetadataJson.TryDeserialize(out InteractionMetadata interactionMetadata))
                {
                    var errorMessage = $"Could not deserialize interaction metadata: {interactionMetadataJson}";
                    _logger.LogWarning(errorMessage);
                    throw new Exception(errorMessage);
                }

                EnrichInteractionMetadata(interactions, document, message, interactionMetadata, review);
                await _interactionsRepository.InsertAsync(interactionMetadata, cancellationToken);
            }
        }

        private ChatHistory GetOrCreateChatHistory(SlackInteractionsQueueItem interactions)
        {
            if (!_chatsHistory.TryGetValue(interactions.ChannelId, out ChatHistory chatHistory))
            {
                var leadershipStyleDescription = LeadershipDescriptions.LeadershipStyleDescriptions[_personality.LeadershipStyle];
                chatHistory = new ChatHistory($"""
                        You are an experienced {_personality.LeadershipStyle}.
                        {leadershipStyleDescription}.
                        You will be reviewing interactions of a development team. 
                        Your goal is to analyze them and extract valuable insights to share with each team member to help them becoming better professionals.
                        Consider both soft and hard skills.
                        Leverage the kernel plugins registered to fetch more information regarding the interactions channel composition and team members data.
                    """);

                chatHistory.AddSystemMessage($"""
                        Channel name: {interactions.ChannelName}    
                        Channel topic: {interactions.ChannelTopic}    
                        Channel purpose: {interactions.ChannelPurpose}
                        For each iteration you will be given an interaction, so as its Sentiment Analysis.
                        You should output the interaction analysis in JSON following this schema:
                        {typeof(InteractionMetadata).BuildDtoDescriptionInJson()}
                    """);

                _chatsHistory.AddOrUpdate(interactions.ChannelId, chatHistory, (_, __) => chatHistory);
            }

            return chatHistory;
        }

        private static void EnrichInteractionMetadata(
            SlackInteractionsQueueItem interactions, 
            TextDocumentInput document, 
            SlackMessage message, 
            InteractionMetadata interactionMetadata,
            AnalyzeSentimentResult sentimentResult)
        {
            interactionMetadata.Source = InteractionSource.Slack;
            interactionMetadata.Text = document.Text;
            interactionMetadata.ChannelId = interactions.ChannelId;
            interactionMetadata.ChannelName = interactions.ChannelName;
            interactionMetadata.TeamMemberEmail = message.FromEmail;
            interactionMetadata.Sentiment = new SentimentResult
            {
                Score = new ConfidenceScore
                {
                    Negative = sentimentResult.DocumentSentiment.ConfidenceScores.Negative,
                    Positive = sentimentResult.DocumentSentiment.ConfidenceScores.Positive,
                    Neutral = sentimentResult.DocumentSentiment.ConfidenceScores.Neutral,
                },
                Sentiment = sentimentResult.DocumentSentiment.Sentiment.ToString(),
                Opinion = string.Join(". ", sentimentResult.DocumentSentiment.Sentences.SelectMany(s => s.Opinions))
            };
        }
    }
}
