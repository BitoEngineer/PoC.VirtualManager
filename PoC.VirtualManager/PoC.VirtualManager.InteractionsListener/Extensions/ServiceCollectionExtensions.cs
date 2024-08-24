using Microsoft.Extensions.DependencyInjection;
using PoC.VirtualManager.InteractionsListener;

namespace PoC.VirtualManager.Interactions.Listener
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInteractionsListener(this IServiceCollection services)
        {
            services.AddHostedService<InteractionsListenerBackgroundService>();

            return services;
        }
    }
}
