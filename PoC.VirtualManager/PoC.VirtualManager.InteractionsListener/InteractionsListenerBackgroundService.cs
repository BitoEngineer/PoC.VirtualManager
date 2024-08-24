using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using PoC.VirtualManager.Interactions.Listener.Extensions;
using PoC.VirtualManager.Interactions.Listener.Models;
using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace PoC.VirtualManager.InteractionsListener
{
    public class InteractionsListenerBackgroundService : BackgroundService
    {
        private readonly Channel<string> _interactionsChannel;
        private readonly Channel<string> _slackInteractionsChannel;
        private readonly Channel<string> _meetingsInteractionsChannel;
        private readonly ILogger<InteractionsListenerBackgroundService> _logger;

        public InteractionsListenerBackgroundService(
            [FromKeyedServices("interactionsChannel")] Channel<string> channel,
            [FromKeyedServices("slackInteractionsChannel")] Channel<string> slackInteractionsChannel,
            [FromKeyedServices("googleMeetsInteractionsChannel")] Channel<string> meetingsInteractionsChannel,
            IChatCompletionService chatCompletionService,
            ILogger<InteractionsListenerBackgroundService> logger,
            StreamWriter writer)
        {
            ArgumentNullException.ThrowIfNull(channel, nameof(channel));
            ArgumentNullException.ThrowIfNull(slackInteractionsChannel, nameof(slackInteractionsChannel));
            ArgumentNullException.ThrowIfNull(meetingsInteractionsChannel, nameof(meetingsInteractionsChannel));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            _interactionsChannel = channel;
            _slackInteractionsChannel = slackInteractionsChannel;
            _meetingsInteractionsChannel = meetingsInteractionsChannel;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() => ListenToSlackInteractionsAsync(cancellationToken), cancellationToken);
            Task.Factory.StartNew(() => ListenToMeetingsInteractionsAsync(cancellationToken), cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task ListenToSlackInteractionsAsync(CancellationToken cancellationToken)
        {
            await foreach (var interaction in _slackInteractionsChannel.Reader.ReadAllAsync(cancellationToken))
            {
                _logger.LogInformation($"Received Slack interaction: {interaction}");
                await _interactionsChannel.Writer.WriteAsync(interaction, cancellationToken);
            }
        }

        private async Task ListenToMeetingsInteractionsAsync(CancellationToken cancellationToken)
        {
            await foreach (var interaction in _meetingsInteractionsChannel.Reader.ReadAllAsync(cancellationToken))
            {
                _logger.LogInformation($"Received Meeting interaction: {interaction}");
                await _interactionsChannel.Writer.WriteAsync(interaction, cancellationToken);
            }
        }

        //TODO isolate this in a SlackStubClient and a GoogleMeetsStubClient
        private readonly Dictionary<string, Type> _promptsFileNamesForJson = new Dictionary<string, Type>()
        {
            { "Prompts/GenerateMeetingTranscriptPrompt.txt", typeof(Meeting) },
        };


        private readonly List<string> _promptsFileNamesForPlainText = new List<string>()
        {
            ,
            "Prompts/GenerateMeetingTranscriptPromptPlainText.txt"
        };
    }
}
