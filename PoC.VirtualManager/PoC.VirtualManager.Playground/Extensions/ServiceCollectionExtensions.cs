using Microsoft.Extensions.DependencyInjection;
using PoC.VirtualManager.Interactions.Slack.Client;
using PoC.VirtualManager.Playground.Apollo;
using PoC.VirtualManager.Slack.Client;
using PoC.VirtualManager.Teams.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Playground.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMockedClientsForApolloPlayground(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ISlackClient, ApolloSlackClient>();
            serviceCollection.AddScoped<ITeamsApiClient, ApolloTeamClient>();

            return serviceCollection;
        }
    }
}
