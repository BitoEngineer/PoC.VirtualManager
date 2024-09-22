using MongoDB.Bson;
using MongoDB.Driver;
using PoC.VirtualManager.Company.Client.Models;
using PoC.VirtualManager.Utils.MongoDb;
using PoC.VirtualManager.Utils.MongoDb.Models;

namespace PoC.VirtualManager.Company.Infrastructure
{
    public interface ITeamsRepository
    {
        Task<Team> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<Team> GetByIdAsync(string id, IClientSessionHandle session, CancellationToken cancellationToken);
        Task<Team> GetByNameAsync(string teamName, CancellationToken cancellationToken);
        Task<Team> InsertAsync(Team team, CancellationToken cancellationToken);
        Task<Team> UpdateAsync(Team team, CancellationToken cancellationToken);
        Task<Team> UpdateAsync(Team team, IClientSessionHandle session, CancellationToken cancellationToken);
    }

    public class TeamsRepository : MongoDbRepository<Team>, ITeamsRepository
    { //TODO refactor - use Utils.MongoDb

        public TeamsRepository(string databaseName, string mongoDbConnectionString)
            : base(new MongoDbSettings
            {
                ConnectionString = mongoDbConnectionString,
                DatabaseName = databaseName,
                CollectionName = "Teams"
            })
        {
        }

        //TODO override InsertAsync and also updateTeamMemberIds on the Teams Repository

        public async Task<Team> GetByNameAsync(string teamName, CancellationToken cancellationToken)
        {
            var filter = Builders<Team>.Filter.Eq(t => t.Name, teamName);

            return await GetByFilterAsync(filter, cancellationToken);
        }
    }
}
