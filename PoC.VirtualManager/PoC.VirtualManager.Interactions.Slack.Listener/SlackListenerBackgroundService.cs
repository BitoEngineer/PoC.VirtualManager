using Microsoft.SemanticKernel.ChatCompletion;
using PoC.VirtualManager.Interactions.Models;
using PoC.VirtualManager.Interactions.Extensions;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace PoC.VirtualManager.Interactions.Slack.Listener
{
    internal class SlackListenerBackgroundService : BackgroundService
    {
        private readonly IChatCompletionService _chatCompletionService;
        private readonly StreamWriter _writer;
        private readonly Channel<string> _interactionsChannel;

        public SlackListenerBackgroundService(IChatCompletionService chatCompletionService,
            StreamWriter writer,
            [FromKeyedServices("interactionsChannel")] Channel<string> interactionsChannel)
        {
            ArgumentNullException.ThrowIfNull(chatCompletionService, nameof(chatCompletionService));
            ArgumentNullException.ThrowIfNull(interactionsChannel, nameof(interactionsChannel));
            ArgumentNullException.ThrowIfNull(writer, nameof(writer));

            _chatCompletionService = chatCompletionService;
            _writer = writer;
            _interactionsChannel = interactionsChannel;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var interaction = await GetInteractionAsPlainTextAsync();
                await _interactionsChannel.Writer.WriteAsync(interaction);
                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        }

        private async Task<string> GetInteractionAsPlainTextAsync()
        {
            var promptToGenerateInteraction = await File.ReadAllTextAsync("Prompts/GenerateChatInteractionsPromptPlainText.txt");
            var interactionStream = _chatCompletionService.GetStreamingChatMessageContentsAsync(promptToGenerateInteraction);
            var fullInteraction = new StringBuilder();

            _writer.WriteLine();
            _writer.WriteLine("--------------------------------------------------------------------------------------------");
            _writer.WriteLine("Team > ");
            await foreach (var tokens in interactionStream)
            {
                _writer.Write(tokens.Content);
                fullInteraction.Append(tokens.Content);
            }

            return fullInteraction.ToString();
        }

        private async Task<(string Json, IInteraction Object)> GetInteractionAsJsonAsync(List<string> promptsKeys)
        {
            var promptToGenerateInteraction = await File.ReadAllTextAsync("Prompts/GenerateChatInteractionsPrompt.txt");
            var interactionJson = (await _chatCompletionService.GetChatMessageContentAsync(promptToGenerateInteraction)).Content;
            if (!interactionJson.TryDeserialize(typeof(Chat), out object interaction))
            {
                throw new Exception("Could not parse response: " + interactionJson);
            }

            return (interactionJson, interaction as IInteraction);
        }
    }
}
