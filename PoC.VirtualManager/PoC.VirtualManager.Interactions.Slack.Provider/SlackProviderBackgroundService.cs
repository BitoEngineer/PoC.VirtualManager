using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PoC.VirtualManager.Interactions.Slack.Client.Models.Nested;
using PoC.VirtualManager.Slack.Client;
using PoC.VirtualManager.Slack.Client.Models.Messaging;
using System.Threading.Channels;

namespace PoC.VirtualManager.Interactions.Slack.Provider
{
    public class SlackProviderBackgroundService : BackgroundService
    {
        private readonly ISlackClient _slackClient;
        private readonly ILogger<SlackProviderBackgroundService> _logger;
        private readonly Channel<SlackFeedbackQueueItem> _feedbackChannel;

        public SlackProviderBackgroundService(
            ISlackClient slackClient,
            ILogger<SlackProviderBackgroundService> logger,
            [FromKeyedServices("slack-feedback-channel")] Channel<SlackFeedbackQueueItem> feedbackChannel)
        {
            ArgumentNullException.ThrowIfNull(feedbackChannel, nameof(feedbackChannel));
            ArgumentNullException.ThrowIfNull(slackClient, nameof(slackClient));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            _feedbackChannel = feedbackChannel;
            _slackClient = slackClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await foreach (var item in _feedbackChannel.Reader.ReadAllAsync(stoppingToken))
                {
                    _logger.LogInformation($"Consumed feedback to be broadcasted");

                    await _slackClient.SendMessageAsync(item.ChannelId, item.Message, stoppingToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        }
    }
}
