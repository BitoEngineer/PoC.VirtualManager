using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using PoC.VirtualManager.Personality.Leadership;
using PoC.VirtualManager.Plugins;
using PoC.VirtualManager.Slack.Client.Models.Messaging;
using PoC.VirtualManager.Teams.Plugins;
using System.Text;
using System.Threading.Channels;

namespace PoC.VirtualManager
{
    public class Brain : BackgroundService
    {
        private readonly IChatCompletionService _chatCompletionService;
        private readonly PersonalityKernelPlugin _personalityKernelPlugin;
        private readonly CompanyKernelPlugin _companyKernelPlugin;
        private readonly TeamKernelPlugin _teamMemberKernelPlugin;
        private readonly Kernel _kernel;
        private readonly ILogger<Brain> _logger;
        private readonly Channel<SlackInteractionsQueueItem> _slackInteractionsChannel;
        private readonly Channel<SlackFeedbackQueueItem> _slackFeedbackChannel;
        private readonly Personality.Personality _personality;
        private readonly StreamWriter _writer;

        public Brain(
            ILogger<Brain> logger,
            Personality.Personality personality,
            IChatCompletionService chatCompletionService,
            Kernel kernel,
            PersonalityKernelPlugin personalityKernelPlugin,
            TeamKernelPlugin teamMemberKernelPlugin,
            CompanyKernelPlugin companyKernelPlugin,
            [FromKeyedServices("slack-interactions-channel")] Channel<SlackInteractionsQueueItem> interactionsChannel,
            [FromKeyedServices("slack-feedback-channel")] Channel<SlackFeedbackQueueItem> feedbackChannel,
            StreamWriter writer)
        {
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ArgumentNullException.ThrowIfNull(personality, nameof(personality));
            ArgumentNullException.ThrowIfNull(chatCompletionService, nameof(chatCompletionService));
            ArgumentNullException.ThrowIfNull(kernel, nameof(kernel));
            ArgumentNullException.ThrowIfNull(personalityKernelPlugin, nameof(personalityKernelPlugin));
            ArgumentNullException.ThrowIfNull(teamMemberKernelPlugin, nameof(teamMemberKernelPlugin));
            ArgumentNullException.ThrowIfNull(companyKernelPlugin, nameof(companyKernelPlugin));
            ArgumentNullException.ThrowIfNull(interactionsChannel, nameof(interactionsChannel));
            ArgumentNullException.ThrowIfNull(feedbackChannel, nameof(feedbackChannel));
            ArgumentNullException.ThrowIfNull(writer, nameof(writer));

            _logger = logger;
            _personality = personality;
            _chatCompletionService = chatCompletionService;
            _kernel = kernel;
            _personalityKernelPlugin = personalityKernelPlugin;
            _companyKernelPlugin = companyKernelPlugin;
            _teamMemberKernelPlugin = teamMemberKernelPlugin;
            _slackInteractionsChannel = interactionsChannel;
            _slackFeedbackChannel = feedbackChannel;
            _writer = writer;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Brain awaken!");
            _kernel.ImportPluginFromObject(_personalityKernelPlugin);
            _kernel.ImportPluginFromObject(_companyKernelPlugin);
            _kernel.ImportPluginFromObject(_teamMemberKernelPlugin);

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Going to sleep, bye bye.");
            _writer.Close();
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var promptSettings = new OpenAIPromptExecutionSettings()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };
            var prompt = await File.ReadAllTextAsync("Prompts/AnalyzeInteractionsPrompt.txt");

            var leadershipStyleDescription = LeadershipDescriptions.LeadershipStyleDescriptions[_personality.LeadershipStyle];
            ChatHistory chatMessages = new ChatHistory($"""
                You are an experienced {_personality.LeadershipStyle}.
                {leadershipStyleDescription}.
                """);
            await foreach (var interaction in _slackInteractionsChannel.Reader.ReadAllAsync(stoppingToken))
            {
                _logger.LogInformation($"Consumed interaction");

                await ProcessInteractionToSlackAsync(promptSettings, prompt, chatMessages, interaction, stoppingToken);
                //await ProcessInteractionToTextAsync(promptSettings, prompt, chatMessages, interaction.Message);
            }
        }

        // TODO:
        // - Isolate this in a SlackProcessorBackgroundService
        // - Add Slack Plugins in order to fetch the channel ID for the individual channels with each team member
        // - The LLM Client should return the SlackFeedbackQueueItem already in JSON, containing all the necessary info for the feedback provider
        private async Task ProcessInteractionToSlackAsync(OpenAIPromptExecutionSettings promptSettings, string prompt, ChatHistory chatMessages, SlackInteractionsQueueItem interaction, CancellationToken stoppingToken)
        {
            var promptWithInteractions = prompt + "\n" + interaction.Message;
            chatMessages.AddUserMessage(promptWithInteractions);
            var feedback = (await _chatCompletionService.GetChatMessageContentAsync(
                            chatMessages,
                            executionSettings: promptSettings,
                            kernel: _kernel)).Content;
            await _slackFeedbackChannel.Writer.WriteAsync(new SlackFeedbackQueueItem
            {
                Message = feedback,
                ChannelId = interaction.ChannelId
            }, stoppingToken);
        }

        private async Task ProcessInteractionToTextAsync(OpenAIPromptExecutionSettings promptSettings, string prompt, ChatHistory chatMessages, string interaction)
        {
            var promptWithInteractions = prompt + "\n" + interaction;
            chatMessages.AddUserMessage(promptWithInteractions);
            var resultStream = _chatCompletionService.GetStreamingChatMessageContentsAsync(
                            chatMessages,
                            executionSettings: promptSettings,
                            kernel: _kernel);

            var fullResponse = new StringBuilder();

            _writer.WriteLine();
            _writer.WriteLine("--------------------------------------------------------------------------------------------");
            _writer.WriteLine("Virtual Manager > ");
            await foreach (var tokens in resultStream)
            {
                _writer.Write(tokens.Content);
                fullResponse.Append(tokens.Content);
            }
            _logger.LogInformation(fullResponse.ToString());
        }
    }
}
