using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Slack.Plugins.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSlackKernelPlugins(this IServiceCollection services)
        {
            services.AddScoped<SlackKernelPlugin>();

            return services;
        }
    }
}
