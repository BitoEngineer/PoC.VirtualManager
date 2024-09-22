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
        public static IServiceCollection AddCompanyRepositories(this IServiceCollection services, 
            string mongoDbConnectionString,
            string databaseName)
        {
            ArgumentNullException.ThrowIfNull(mongoDbConnectionString, nameof(mongoDbConnectionString));
            ArgumentNullException.ThrowIfNull(databaseName, nameof(databaseName));

            services.AddSingleton<ITeamsRepository, TeamsRepository>(
                _ => new TeamsRepository(
                    databaseName: databaseName,
                    mongoDbConnectionString: mongoDbConnectionString)
            );
            services.AddSingleton<ITeamMembersRepository, TeamMembersRepository>(
                serviceProvider => new TeamMembersRepository(
                    databaseName: databaseName,
                    mongoDbConnectionString: mongoDbConnectionString,
                    teamsRepository: serviceProvider.GetRequiredService<ITeamsRepository>())
            );

            return services;
        }
    }
}
