using PoC.VirtualManager.Teams.Client.Models;

namespace PoC.VirtualManager.Teams.Client
{
    public interface ITeamsApiClient
    {
        Task<PersonalityTraits> GetPersonalityTraitMeaningAsync(string personalityTrait);
        Task<Team> GetTeamAsync(string teamId);
        Task<TeamMember> GetTeamMemberAsync(string memberName);
    }
}
