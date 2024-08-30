using Microsoft.Extensions.DependencyInjection;
using PoC.VirtualManager.Interactions.Slack.Listener.Caches;
using PoC.VirtualManager.Slack.Client.Extensions;
using PoC.VirtualManager.Slack.Client.Models.Messaging;
using System.Threading.Channels;

namespace PoC.VirtualManager.Interactions.Slack.Listener.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSlackInteractionsListener(this IServiceCollection services, string accessToken)
        {
            services.AddSlackClient(accessToken);
            services.AddKeyedSingleton("slack-interactions-channel", Channel.CreateUnbounded<SlackInteractionsQueueItem>());
            services.AddSingleton<IUsersCache, UsersCache>();

            services.AddHostedService<SlackListenerBackgroundService>();

            return services;
        }
    }
}
