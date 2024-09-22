using PoC.VirtualManager.Company.Client.Models;
using PoC.VirtualManager.Company.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.CreateTeams
{
    public class CreateApolloTeam
    {
        private const string DatabaseName = "Company";
        private readonly ITeamsRepository _teamRepository;
        private readonly ITeamMembersRepository _teamMembersRepository;

        public CreateApolloTeam()
        {
            _teamRepository = new TeamsRepository(
                databaseName: DatabaseName,
                mongoDbConnectionString: Environment.GetEnvironmentVariable("VirtualManager_MongoDb_ConnectionString"));
            
            _teamMembersRepository = new TeamMembersRepository(
               databaseName: DatabaseName,
               mongoDbConnectionString: Environment.GetEnvironmentVariable("VirtualManager_MongoDb_ConnectionString"),
               teamsRepository: _teamRepository);
        }

        public async Task RunAsync()
        {
            var apolloTeam = new Team
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Department = "IT",
                Description = "Data ingestion team. Responsible for the streaming pipeline to ingest vehicles tracking data.",
                DomainExpertise = "Tracking data, locations, DaemonWorker, TripsWorker, Locations API, etc",
                Name = "Apollo",
                TechnicalExpertise = "C#, .NET, React",
                Methodoly = "Scrum",
            };

            var apolloTeamWithId = await _teamRepository.InsertAsync(apolloTeam, default);

            var nunoCardoso = new TeamMember
            {

            };
            var nunoCardosoWithId = await _teamMembersRepository.InsertAsync(nunoCardoso, default);

            var fabioAnselmo = new TeamMember
            {
                TeamId = apolloTeamWithId.Id,
                TechLeadId = nunoCardosoWithId.Id,
            };
            var fabioAnselmoWithId = await _teamMembersRepository.InsertAsync(fabioAnselmo, default);

            var marcoAgostinho = new TeamMember
            {
                TeamId = apolloTeamWithId.Id,
                TechLeadId = fabioAnselmoWithId.Id,
            };
            //TODO
        }
    }
}
