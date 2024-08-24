using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace PoC.VirtualManager.Interactions.Slack.Listener.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSlackInteractionsListener(this IServiceCollection services)
        {
            services.AddHostedService<SlackListenerBackgroundService>();

            return services;
        }
    }
}
