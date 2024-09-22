using MongoDB.Bson;
using MongoDB.Driver;
using PoC.VirtualManager.Company.Client.Models;

namespace PoC.VirtualManager.Company.Infrastructure
{
    public interface ITeamsRepository
    {
        Task<Team> GetTeamByIdAsync(string teamId);
        Task<Team> GetTeamByNameAsync(string teamName);
        Task<Team> InsertTeamAsync(Team team, CancellationToken cancellationToken);
        Task<Team> UpdateTeamAsync(Team team);
        Task<TeamMember> GetTeamMemberByIdAsync(string teamMemberId);
        Task<TeamMember> GetTeamMemberByNameAsync(string teamName);
        Task<TeamMember> InsertTeamMemberAsync(TeamMember teamMember, CancellationToken cancellationToken);
        Task<TeamMember> UpsertTeamMemberAsync(TeamMember teamMember);
    }

    public class TeamsRepository : ITeamsRepository
    { //TODO refactor - use Utils.MongoDb
        private const string DatabaseName = "Company";
        private const string TeamsCollectionName = "Teams";
        private const string TeamMembersCollectionName = "TeamMembers";

        private readonly MongoClient _client;
        private readonly Lazy<IMongoDatabase> _database;
        private readonly Lazy<Task<IMongoCollection<Team>>> _teamsCollectionInitializer;
        private readonly Lazy<Task<IMongoCollection<TeamMember>>> _teamMembersCollectionInitializer;

        private IMongoCollection<Team> _teamsCollection => _teamsCollectionInitializer.Value.Result;
        private IMongoCollection<TeamMember> _teamMembersCollection => _teamMembersCollectionInitializer.Value.Result;

        public TeamsRepository(string mongoDbConnectionString)
            : this(MongoClientSettings.FromConnectionString(mongoDbConnectionString))
        {
            ArgumentNullException.ThrowIfNull(mongoDbConnectionString, nameof(mongoDbConnectionString));
        }

        public TeamsRepository(MongoClientSettings settings)
            : this(new MongoClient(ApplyServerApi(settings)))
        {
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));
        }

        public TeamsRepository(MongoClient client)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            _client = client;
            _database = new Lazy<IMongoDatabase>(() => _client.GetDatabase(DatabaseName));
            _teamsCollectionInitializer = new Lazy<Task<IMongoCollection<Team>>>(GetOrCreateCollectionAsync<Team>);
            _teamMembersCollectionInitializer = new Lazy<Task<IMongoCollection<TeamMember>>>(GetOrCreateCollectionAsync<TeamMember>);
        }

        public Task<Team> GetTeamByIdAsync(string teamId)
        {
            var filter = Builders<Team>.Filter.Eq(t => t.Id, teamId);

            return _teamsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public Task<Team> GetTeamByNameAsync(string teamName)
        {
            var filter = Builders<Team>.Filter.Eq(t => t.Name, teamName);

            return _teamsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public Task<TeamMember> GetTeamMemberByIdAsync(string teamMemberId)
        {
            var filter = Builders<TeamMember>.Filter.Eq(t => t.Id, teamMemberId);

            return _teamMembersCollection.Find(filter).FirstOrDefaultAsync();
        }

        public Task<TeamMember> GetTeamMemberByNameAsync(string teamName)
        {
            var filter = Builders<TeamMember>.Filter.Eq(t => t.Name, teamName);

            return _teamMembersCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<Team> InsertTeamAsync(Team team, CancellationToken cancellationToken)
        {
            team.CreatedAt = DateTime.Now;
            team.UpdatedAt = DateTime.Now;
            await _teamsCollection.InsertOneAsync(team, new InsertOneOptions
            {
            }, cancellationToken);

            return team;
        }

        public async Task<TeamMember> InsertTeamMemberAsync(TeamMember teamMember, CancellationToken cancellationToken)
        {
            teamMember.CreatedAt = DateTime.Now;
            teamMember.UpdatedAt = DateTime.Now;
            await _teamMembersCollection.InsertOneAsync(teamMember, new InsertOneOptions
            {
            }, cancellationToken);

            return teamMember;
        }

        public async Task<Team> UpdateTeamAsync(Team team)
        {
            var filter = Builders<Team>.Filter.Eq(t => t.Id, team.Id);

            team.UpdatedAt = DateTime.UtcNow;
            var result = await _teamsCollection.ReplaceOneAsync(
                filter,
                team,
                new ReplaceOptions { IsUpsert = false }
            );
            
            return team;
        }

        public async Task<TeamMember> UpsertTeamMemberAsync(TeamMember teamMember)
        {
            using var session = await _client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                var filter = Builders<TeamMember>.Filter.Eq(t => t.Id, teamMember.Id);

                var result = await _teamMembersCollection.ReplaceOneAsync(
                    session,
                    filter,
                    teamMember,
                    new ReplaceOptions { IsUpsert = true }
                );

                if (result.UpsertedId != BsonNull.Value)
                {
                    teamMember.Id = result.UpsertedId.AsGuid.ToString();
                }

                var team = await GetTeamByIdAsync(teamMember.TeamId);

                if (!team.TeamMembersIds.Contains(teamMember.Id))
                {
                    team.TeamMembersIds.Add(teamMember.Id);
                    await UpdateTeamAsync(team);
                }

                await session.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                // Log the error and rethrow or return a meaningful error response
                throw new Exception("Failed to upsert team member", ex);
            }

            return teamMember;
        }

        private async Task<IMongoCollection<T>> GetOrCreateCollectionAsync<T>()
        {
            var teamsCollection = _database.Value.GetCollection<T>(TeamsCollectionName);

            if (teamsCollection == null)
            {
                await _database.Value.CreateCollectionAsync(TeamsCollectionName);
            }

            return teamsCollection ?? _database.Value.GetCollection<T>(TeamsCollectionName);
        }

        private static MongoClientSettings ApplyServerApi(MongoClientSettings settings)
        {
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            return settings;
        }
    }
}
