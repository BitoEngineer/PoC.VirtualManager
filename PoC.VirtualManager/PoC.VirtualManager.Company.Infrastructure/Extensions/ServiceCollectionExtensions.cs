using Microsoft.Extensions.DependencyInjection;
using PoC.VirtualManager.Company.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Company.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTeamsRespositories(this IServiceCollection services, string mongoDbConnectionString)
        {
            services.AddSingleton<ITeamsRepository, TeamsRepository>(_ => new TeamsRepository(mongoDbConnectionString));

            return services;
        }
    }
}
