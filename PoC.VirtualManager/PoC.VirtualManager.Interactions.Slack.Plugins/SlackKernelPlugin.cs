using Microsoft.SemanticKernel;
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

        [KernelFunction("get_channel_info")]
        [Description("Based on the channel id obtains all the Slack channel related info")]
        [return: Description("The channel info")]
        public async Task<string> GetChannelInfo(string channelId)
        {
            var getChannelInfoTask = _slackApiClient.GetConversationInfoAsync(channelId, default);
            var getChannelMembersTask = _slackApiClient.GetConversationMembersAsync(channelId, default);
            await Task.WhenAll(getChannelInfoTask, getChannelMembersTask);
            return $"Channel Info: \n{getChannelInfoTask.Result}\nChannel Members: \n{getChannelMembersTask.Result}\n";
        }
    }
}
