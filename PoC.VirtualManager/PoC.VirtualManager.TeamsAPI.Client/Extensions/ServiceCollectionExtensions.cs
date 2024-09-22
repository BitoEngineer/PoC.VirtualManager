using Microsoft.Extensions.DependencyInjection;

namespace PoC.VirtualManager.Teams.Client.Extensions
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
