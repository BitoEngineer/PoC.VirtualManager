using PoC.VirtualManager.Interactions.Slack.Client.Models;
using PoC.VirtualManager.Interactions.Slack.Listener.Caches;
using PoC.VirtualManager.Slack.Client.Models.Messaging;
using System.Text;
using System.Text.RegularExpressions;

namespace PoC.VirtualManager.Interactions.Slack.Listener.Extensions
{
    public static class SlackMessagesExtensions
    {
        public static async Task<SlackInteractionsQueueItem> ToSlackInteractionsQueueItemAsync(this List<ConversationMessage> messages,
            DateTimeOffset from,
            DateTimeOffset to,
            IUsersCache usersCache,
            Client.Models.Nested.Channel channel,
            CancellationToken stoppingToken)
        {
            return new SlackInteractionsQueueItem
            {
                ChannelId = channel.Id,
                ChannelName = channel.Name,
                ChannelPurpose = channel.Purpose?.Value,
                ChannelTopic = channel.Topic?.Value,
                Messages = (await Task.WhenAll(messages.Select(async message =>
                {
                    var user = await usersCache.GetUserAsync(message.User, stoppingToken);
                    return new SlackMessage
                    {
                        FromName = user.Name,
                        FromEmail = user?.Profile?.Email,
                        Timestamp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(message.Ts.Split(".")[0])).DateTime,
                        Message = await ReplaceAllUserIdsWithUserNames(message.Text, usersCache, stoppingToken)
                    };
                }))).ToList()
            };
        }

        private static async Task<string> ReplaceAllUserIdsWithUserNames(
            string message, 
            IUsersCache usersCache,
            CancellationToken stoppingToken)
        {
            string pattern = @"\<@([A-Z0-9]+)\>";
            HashSet<string> processedUserIds = new HashSet<string>();

            MatchCollection matches = Regex.Matches(message, pattern);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    string userId = match.Groups[1].Value;

                    if (!processedUserIds.Contains(userId))
                    {
                        var userName = (await usersCache.GetUserAsync(userId, stoppingToken))?.RealName ?? "Unknown User";

                        message = Regex.Replace(message, $@"\<@{userId}\>", userName);

                        processedUserIds.Add(userId);
                    }
                }
            }

            return message;
        }
    }
}
