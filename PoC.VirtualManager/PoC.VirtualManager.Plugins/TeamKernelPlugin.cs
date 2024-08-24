using Microsoft.SemanticKernel;
using PoC.VirtualManager.Teams.Client;
using PoC.VirtualManager.Teams.Client.Models;
using System.ComponentModel;

namespace PoC.VirtualManager.Plugins
{
    public class TeamKernelPlugin
    {
        private readonly ITeamsApiClient _teamsApiClient;

        public TeamKernelPlugin(ITeamsApiClient teamsApiClient)
        {
            ArgumentNullException.ThrowIfNull(teamsApiClient, nameof(teamsApiClient));

            _teamsApiClient = teamsApiClient;
        }

        [KernelFunction("get_team_info")]
        [Description("Provides information related to the team composition, domain and focus")]
        [return: Description("Team information")]
        public async Task<Team> GetTeamInfo(string teamName)
        {
            return await _teamsApiClient.GetTeamAsync(teamName);
        }

        [KernelFunction("get_team_member_info")]
        [Description("Provides information related to a team member, including personality traits score, background, etc")]
        [return: Description("Team member info")]
        public async Task<TeamMember> GetTeamMemberInfo(string memberName)
        {
            return await _teamsApiClient.GetTeamMemberAsync(memberName);
        }
    }
}
