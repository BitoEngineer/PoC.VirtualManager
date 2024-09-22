using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PoC.VirtualManager.Slack.Client.Models;

namespace PoC.VirtualManager.Interactions.Slack.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSlackClient(this IServiceCollection services, string accessToken)
        {
            services.AddSingleton(new SlackSettings
            {
                AccessToken = accessToken,
            });

            services.AddHttpClient();
            services.TryAddScoped<ISlackClient, SlackClient>();

            return services;
        }
    }
}
