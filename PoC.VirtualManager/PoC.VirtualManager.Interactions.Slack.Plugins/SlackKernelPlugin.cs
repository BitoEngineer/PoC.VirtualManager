using Microsoft.SemanticKernel;
using PoC.VirtualManager.Interactions.Slack.Client;
using PoC.VirtualManager.Slack.Client;
using System.ComponentModel;

namespace PoC.VirtualManager.Interactions.Slack.Plugins
{
    public class SlackKernelPlugin
    {
        private readonly ISlackClient _slackApiClient;

        public SlackKernelPlugin(ISlackClient slackApiClient)
        {
            ArgumentNullException.ThrowIfNull(slackApiClient, nameof(slackApiClient));

            _slackApiClient = slackApiClient;
        }

        [KernelFunction("get_channel_members")]
        [Description("Based on the channel id obtains all the Slack channel members")]
        [return: Description("The channel info")]
        public async Task<string> GetChannelMembers(string channelId)
        {
            var getChannelMembers = await _slackApiClient.GetConversationMembersAsync(channelId, default);
            var allUsersInfo = (await _slackApiClient.ListUsersAsync(default))
                .Members
                .ToDictionary(m => m.Id, m => m);

            return string.Join("\n", getChannelMembers.Members.Select(id =>
            {
                var memberInfo = allUsersInfo[id];
                return $"Id: {id} | Name: {memberInfo.Name} | Email: {memberInfo.Profile.Email} | Team: {memberInfo.Profile.Team} | Title: {memberInfo.Profile.Title} | Status: {memberInfo.Profile.StatusText}";
            }));
        }
    }
}
