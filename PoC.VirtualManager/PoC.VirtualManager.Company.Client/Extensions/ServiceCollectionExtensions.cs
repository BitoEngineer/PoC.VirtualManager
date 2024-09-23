using Microsoft.Extensions.DependencyInjection;
using PoC.VirtualManager.Company.Infrastructure;

namespace PoC.VirtualManager.Company.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTeamsClient(this IServiceCollection services)
        {
            services.AddScoped<ITeamsApiClient, TeamsApiClient>();

            return services;
        }
    }
}
