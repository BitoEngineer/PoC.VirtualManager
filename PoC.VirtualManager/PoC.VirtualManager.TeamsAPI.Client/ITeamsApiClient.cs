using PoC.VirtualManager.Teams.Client.Models;

namespace PoC.VirtualManager.Teams.Client
{
    public interface ITeamsApiClient
    {
        Task<Trait> GetPersonalityTraitMeaningAsync(string personalityTrait);
        Task<Team> GetTeamAsync(string teamId);
        Task<TeamMember> GetTeamMemberByEmailAsync(string memberName);
    }

    public class TeamsApiClient : ITeamsApiClient
    {
        public Task<Trait> GetPersonalityTraitMeaningAsync(string personalityTrait)
        {
            throw new NotImplementedException();
        }

        public Task<Team> GetTeamAsync(string teamId)
        {
            throw new NotImplementedException();
        }

        public Task<TeamMember> GetTeamMemberByEmailAsync(string memberName)
        {
            throw new NotImplementedException();
        }
    }
}
