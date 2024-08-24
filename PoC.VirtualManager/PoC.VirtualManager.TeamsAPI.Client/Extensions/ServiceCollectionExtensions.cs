using Microsoft.Extensions.DependencyInjection;
using PoC.VirtualManager.Teams.Client.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Teams.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTeamsStubClient(this IServiceCollection services)
        {
            services.AddScoped<ITeamsApiClient, TeamsApiStubClient>();

            return services;
        }
    }
}
