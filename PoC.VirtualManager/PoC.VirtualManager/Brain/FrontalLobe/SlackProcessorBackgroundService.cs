using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using PoC.VirtualManager.Brain.FrontalLobe.Personality.Leadership;
using PoC.VirtualManager.Plugins;
using PoC.VirtualManager.Slack.Client.Models.Messaging;
using PoC.VirtualManager.Company.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Brain.FrontalLobe
{
    public class SlackProcessorBackgroundService : BackgroundService
    {
        private readonly IChatCompletionService _chatCompletionService;
        private readonly Kernel _kernel;
        private readonly ILogger<SlackProcessorBackgroundService> _logger;
        private readonly Channel<SlackInteractionsQueueItem> _slackInteractionsChannel;
        private readonly Channel<SlackFeedbackQueueItem> _slackFeedbackChannel;
        private readonly Personality.Personality _personality;

        public SlackProcessorBackgroundService(
            ILogger<SlackProcessorBackgroundService> logger,
            Personality.Personality personality,
            IChatCompletionService chatCompletionService,
            Kernel kernel,
            [FromKeyedServices("slack-interactions-channel")] Channel<SlackInteractionsQueueItem> interactionsChannel,
            [FromKeyedServices("slack-feedback-channel")] Channel<SlackFeedbackQueueItem> feedbackChannel,
            StreamWriter writer)
        {
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ArgumentNullException.ThrowIfNull(personality, nameof(personality));
            ArgumentNullException.ThrowIfNull(chatCompletionService, nameof(chatCompletionService));
            ArgumentNullException.ThrowIfNull(kernel, nameof(kernel));
            ArgumentNullException.ThrowIfNull(interactionsChannel, nameof(interactionsChannel));
            ArgumentNullException.ThrowIfNull(feedbackChannel, nameof(feedbackChannel));
            ArgumentNullException.ThrowIfNull(writer, nameof(writer));

            _logger = logger;
            _personality = personality;
            _chatCompletionService = chatCompletionService;
            _kernel = kernel;
            _slackInteractionsChannel = interactionsChannel;
            _slackFeedbackChannel = feedbackChannel;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Brain awaken!");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Going to sleep, bye bye.");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var promptSettings = new OpenAIPromptExecutionSettings()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions //TODO check if Plugins are being invoked
            };
            var prompt = await File.ReadAllTextAsync("Prompts/AnalyzeInteractionsPrompt.txt");

            var leadershipStyleDescription = Personality.Leadership.LeadershipDescriptions.LeadershipStyleDescriptions[_personality.LeadershipStyle];
            ChatHistory chatMessages = new ChatHistory($"""
                You are an experienced {_personality.LeadershipStyle}.
                {leadershipStyleDescription}.
                """);
            await foreach (var interaction in _slackInteractionsChannel.Reader.ReadAllAsync(stoppingToken))
            {
                _logger.LogInformation($"Consumed interaction");

                await ProcessInteractionToSlackAsync(promptSettings, prompt, chatMessages, interaction, stoppingToken);
            }
        }

        // TODO:
        // - Add Slack Plugins in order to fetch the channel ID for the individual channels with each team member
        // - The LLM Client should return the SlackFeedbackQueueItem already in JSON, containing all the necessary info for the feedback provider
        private async Task ProcessInteractionToSlackAsync(OpenAIPromptExecutionSettings promptSettings, 
            string prompt, 
            ChatHistory chatMessages, 
            SlackInteractionsQueueItem interaction, 
            CancellationToken stoppingToken)
        {
            var promptWithInteractions = prompt + "\n" + string.Join("\n", interaction.Messages.Select(m => m.ToString()));
            chatMessages.AddUserMessage(promptWithInteractions);
            var feedback = (await _chatCompletionService.GetChatMessageContentAsync(
                            chatMessages,
                            executionSettings: promptSettings,
                            kernel: _kernel)).Content;

            //TODO link with Teams models
            await _slackFeedbackChannel.Writer.WriteAsync(new SlackFeedbackQueueItem
            {
                Message = feedback,
                ChannelId = interaction.ChannelId,
                
            }, stoppingToken);
        }
    }
}
