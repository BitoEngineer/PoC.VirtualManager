using PoC.VirtualManager.Teams.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Teams.Client.Stubs
{
    internal class TeamsApiStubClient : ITeamsApiClient
    {
        private static Lazy<Team> _team = new Lazy<Team>(() => JsonSerializer.Deserialize<Team>(File.ReadAllText("Stubs/AzureDevOpsCoreTeam.json")));
        private static Lazy<PersonalityTraits> _traits = new Lazy<PersonalityTraits>(() => JsonSerializer.Deserialize<PersonalityTraits>(File.ReadAllText("Stubs/PersonalityTraits.json")));

        public Task<PersonalityTraits> GetPersonalityTraitMeaningAsync(string personalityTrait)
        {//TODO fix
            return Task.FromResult(_traits.Value);
        }

        public Task<Team> GetTeamAsync(string teamId)
        {
            return Task.FromResult(default(Team));
            //return Task.FromResult(_team.Value);
        }

        public Task<TeamMember> GetTeamMemberAsync(string memberName)
        {
            //TODO
            return Task.FromResult(default(TeamMember));
            //return Task.FromResult(_team.Value.Members.FirstOrDefault(x => x.Name == memberName));
        }
    }
}
