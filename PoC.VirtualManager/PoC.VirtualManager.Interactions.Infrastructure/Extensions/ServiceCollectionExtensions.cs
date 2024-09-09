using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInteractionsRepositories(this IServiceCollection services, string mongoDbConnectionString)
        {
            services.AddSingleton<IInteractionsRepository, InteractionsRepository>(_ => new InteractionsRepository(mongoDbConnectionString));

            return services;
        }
    }
}
