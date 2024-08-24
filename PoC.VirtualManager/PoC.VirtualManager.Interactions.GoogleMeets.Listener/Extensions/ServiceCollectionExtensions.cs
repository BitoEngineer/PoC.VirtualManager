using Microsoft.Extensions.DependencyInjection;
using PoC.VirtualManager.Interactions.GoogleMeets.Listener.Stub;

namespace PoC.VirtualManager.Interactions.GoogleMeets.Listener.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGoogleMeetsInteractionsListener(this IServiceCollection services)
        {
            services.AddHostedService<GoogleMeetsListenerBackgroundService>();

            return services;
        }
    }
}
