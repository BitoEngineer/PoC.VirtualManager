using MongoDB.Bson;
using MongoDB.Driver;
using PoC.VirtualManager.Company.Client.Models;
using PoC.VirtualManager.Utils.MongoDb;
using PoC.VirtualManager.Utils.MongoDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Company.Infrastructure
{
    public interface ITeamMembersRepository
    {
        Task<TeamMember> InsertAsync(TeamMember item, CancellationToken cancellationToken);
        Task<TeamMember> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<TeamMember> GetByNameAsync(string teamMemberName, CancellationToken cancellationToken);
    }

    public class TeamMembersRepository : MongoDbRepository<TeamMember>, ITeamMembersRepository
    {
        private readonly ITeamsRepository _teamsRepository;

        public TeamMembersRepository(string databaseName, string mongoDbConnectionString, ITeamsRepository teamsRepository) 
            : base(new MongoDbSettings
            {
                ConnectionString = mongoDbConnectionString,
                DatabaseName = databaseName,
                CollectionName = "TeamMembers"
            })
        {
            ArgumentNullException.ThrowIfNull(teamsRepository, nameof(teamsRepository));

            _teamsRepository = teamsRepository;
        }

        public override async Task<TeamMember> InsertAsync(TeamMember teamMember,
            CancellationToken cancellationToken)
        {
            var session = await Client.StartSessionAsync();
            session.StartTransaction(new TransactionOptions(
                readConcern: ReadConcern.Snapshot,
                writeConcern: WriteConcern.WMajority,
                readPreference: ReadPreference.Primary
            ));

            try
            {
                teamMember.CreatedAt = DateTime.Now;
                teamMember.UpdatedAt = DateTime.Now;
                await (await GetOrCreateCollectionAsync(cancellationToken))
                    .InsertOneAsync(session, teamMember, new InsertOneOptions{}, cancellationToken);

                var team = await _teamsRepository.GetByIdAsync(teamMember.TeamId, session, cancellationToken);
                if (!team.TeamMembersIds.Contains(teamMember.Id))
                {
                    team.TeamMembersIds.Add(teamMember.Id);
                    await _teamsRepository.UpdateAsync(team, session, cancellationToken);
                }

                await session.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                throw;
            }
            finally
            {
                session.Dispose();
            }

            return teamMember;
        }

        public async Task<TeamMember> GetByNameAsync(string teamMemberName, CancellationToken cancellationToken)
        {
            var filter = Builders<TeamMember>.Filter.Eq(t => t.Name, teamMemberName);

            return await GetByFilterAsync(filter, cancellationToken);
        }
    }
}
