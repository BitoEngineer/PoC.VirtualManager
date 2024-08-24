using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using PoC.VirtualManager.Personality.Leadership;
using PoC.VirtualManager.Plugins;
using System.Text;
using System.Threading.Channels;

namespace PoC.VirtualManager
{
    public class Brain : BackgroundService
    {
        private readonly IChatCompletionService _chatCompletionService;
        private readonly PersonalityKernelPlugin _personalityKernelPlugin;
        private readonly TeamKernelPlugin _teamMemberKernelPlugin;
        private readonly Kernel _kernel;
        private readonly ILogger<Brain> _logger;
        private readonly Channel<string> _channel;
        private readonly Personality.Personality _personality;
        private readonly StreamWriter _writer;

        public Brain(
            ILogger<Brain> logger,
            Personality.Personality personality,
            IChatCompletionService chatCompletionService,
            Kernel kernel,
            PersonalityKernelPlugin personalityKernelPlugin,
            TeamKernelPlugin teamMemberKernelPlugin,
            [FromKeyedServices("interactionsChannel")] Channel<string> channel,
            StreamWriter writer)
        {
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ArgumentNullException.ThrowIfNull(personality, nameof(personality));
            ArgumentNullException.ThrowIfNull(chatCompletionService, nameof(chatCompletionService));
            ArgumentNullException.ThrowIfNull(kernel, nameof(kernel));
            ArgumentNullException.ThrowIfNull(personalityKernelPlugin, nameof(personalityKernelPlugin));
            ArgumentNullException.ThrowIfNull(teamMemberKernelPlugin, nameof(teamMemberKernelPlugin));
            ArgumentNullException.ThrowIfNull(channel, nameof(channel));
            ArgumentNullException.ThrowIfNull(writer, nameof(writer));

            _logger = logger;
            _personality = personality;
            _chatCompletionService = chatCompletionService;
            _kernel = kernel;
            _personalityKernelPlugin = personalityKernelPlugin;
            _teamMemberKernelPlugin = teamMemberKernelPlugin;
            _channel = channel;
            _writer = writer;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Brain awaken!");
            _kernel.ImportPluginFromObject(_personalityKernelPlugin);
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
            await foreach (var interaction in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                _logger.LogInformation($"Consumed interaction");

                await ProcessInteractionAsync(promptSettings, prompt, chatMessages, interaction);
            }
        }

        private async Task ProcessInteractionAsync(OpenAIPromptExecutionSettings promptSettings, string prompt, ChatHistory chatMessages, string interaction)
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
