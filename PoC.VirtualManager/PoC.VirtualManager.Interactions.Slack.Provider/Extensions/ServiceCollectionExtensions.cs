using Microsoft.Extensions.DependencyInjection;
using PoC.VirtualManager.Slack.Client.Models.Messaging;
using System.Threading.Channels;

namespace PoC.VirtualManager.Interactions.Slack.Broadcaster.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSlackFeedbackProvider(this IServiceCollection services)
        {
            services.AddKeyedSingleton("slack-feedback-channel", Channel.CreateUnbounded<SlackFeedbackQueueItem>());

            services.AddHostedService<SlackProviderBackgroundService>();

            return services;
        }
    }
}
