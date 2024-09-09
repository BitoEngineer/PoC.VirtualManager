using PoC.VirtualManager.Teams.Client.Models;
using PoC.VirtualManager.Teams.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.CreateTeams
{
    public class CreateApolloTeam
    {
        private readonly ITeamsRepository _repository = 
            new TeamsRepository(Environment.GetEnvironmentVariable("VirtualManager_MongoDb_ConnectionString"));
        
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

            var apolloTeamWithId = await _repository.InsertTeamAsync(apolloTeam, default);

            var nunoCardoso = new TeamMember
            {

            };
            var nunoCardosoWithId = await _repository.InsertTeamMemberAsync(nunoCardoso, default);

            var fabioAnselmo = new TeamMember
            {
                TeamId = apolloTeamWithId.Id,
                TechLeadId = nunoCardosoWithId.Id,
            };
            var fabioAnselmoWithId = await _repository.InsertTeamMemberAsync(fabioAnselmo, default);

            var marcoAgostinho = new TeamMember
            {
                TeamId = apolloTeamWithId.Id,
                TechLeadId = fabioAnselmoWithId.Id,
            };
            //TODO
        }
    }
}
