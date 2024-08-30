using Microsoft.Extensions.DependencyInjection;
using PoC.VirtualManager.Teams.Client.Stubs;
using PoC.VirtualManager.Teams.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Plugins
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTeamsKernelPlugins(this IServiceCollection services)
        {
            services.AddScoped<PersonalityKernelPlugin>();
            services.AddScoped<CompanyKernelPlugin>();
            services.AddScoped<TeamKernelPlugin>();

            return services;
        }
    }
}
