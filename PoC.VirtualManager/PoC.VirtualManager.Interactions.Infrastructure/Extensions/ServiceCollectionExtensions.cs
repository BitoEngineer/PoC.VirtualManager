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
            var databaseName = "Interactions";

            services.AddSingleton<IInteractionsMetadataRepository, InteractionsMetadataRepository>(
                _ => new InteractionsMetadataRepository(databaseName, mongoDbConnectionString)
            );

            return services;
        }
    }
}
