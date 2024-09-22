using PoC.VirtualManager.Teams.Client;
using PoC.VirtualManager.Teams.Client.Models;
using System.Reflection;
using System.Text.Json;

namespace PoC.VirtualManager.Playground.Apollo
{
    public class ApolloTeamClient : ITeamsApiClient
    {
        private static Lazy<Team> _team = new Lazy<Team>(
            () => JsonSerializer.Deserialize<Team>(File.ReadAllText("Apollo/Apollo.json"))
        );

        private static Lazy<PersonalityTraits> _traits = new Lazy<PersonalityTraits>(
            () => JsonSerializer.Deserialize<PersonalityTraits>(File.ReadAllText("Apollo/PersonalityTraits.json"))
        );

        private static Lazy<TeamMember[]> _teamMembers = new Lazy<TeamMember[]>(
            () => new TeamMember[]
            {
                JsonSerializer.Deserialize<TeamMember>(File.ReadAllText("Apollo/TeamMembers/Marco_Agostinho.json")),
                JsonSerializer.Deserialize<TeamMember>(File.ReadAllText("Apollo/TeamMembers/Carlos_Romana.json")),
                JsonSerializer.Deserialize<TeamMember>(File.ReadAllText("Apollo/TeamMembers/Diogo_Viana.json")),
                JsonSerializer.Deserialize<TeamMember>(File.ReadAllText("Apollo/TeamMembers/Fabio_Anselmo.json")),
                JsonSerializer.Deserialize<TeamMember>(File.ReadAllText("Apollo/TeamMembers/Joaquim_Tapadas.json")),
                JsonSerializer.Deserialize<TeamMember>(File.ReadAllText("Apollo/TeamMembers/Rodrigo_Correia.json")),
                JsonSerializer.Deserialize<TeamMember>(File.ReadAllText("Apollo/TeamMembers/Tahira_Vissaram.json"))
            }
        );

        public Task<Trait> GetPersonalityTraitMeaningAsync(string personalityTrait)
        {
            return Task.FromResult(GetTraitByName(personalityTrait, _traits.Value));
        }

        public Task<Team> GetTeamAsync(string teamId)
        {
            return Task.FromResult(_team.Value);
        }

        public Task<TeamMember> GetTeamMemberByEmailAsync(string memberEmail)
        {
            return Task.FromResult(_teamMembers.Value.FirstOrDefault(t => t.Email == memberEmail));
        }

        private static Trait GetTraitByName(string traitName, PersonalityTraits traits)
        {
            PropertyInfo property = typeof(PersonalityTraits).GetProperty(traitName);

            if (property == null)
            {
                throw new ArgumentException("Invalid trait name");
            }

            if (!typeof(Trait).IsAssignableFrom(property.PropertyType))
            {
                throw new ArgumentException("Property is not of type Trait");
            }

            return (Trait)property.GetValue(traits);
        }
    }
}
