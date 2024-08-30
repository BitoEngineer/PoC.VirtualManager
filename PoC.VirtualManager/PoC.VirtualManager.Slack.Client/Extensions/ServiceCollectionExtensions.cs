using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PoC.VirtualManager.Slack.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Slack.Client.Extensions
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
