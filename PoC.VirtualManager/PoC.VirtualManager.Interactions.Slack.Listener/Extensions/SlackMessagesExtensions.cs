using PoC.VirtualManager.Interactions.Slack.Client.Models;
using PoC.VirtualManager.Interactions.Slack.Listener.Caches;
using System.Text;
using System.Text.RegularExpressions;

namespace PoC.VirtualManager.Interactions.Slack.Listener.Extensions
{
    public static class SlackMessagesExtensions
    {
        public static async Task<string> ToPlainTextInteractionsAsync(this List<ConversationMessage> messages,
            DateTimeOffset from,
            DateTimeOffset to,
            IUsersCache usersCache,
            Client.Models.Nested.Channel channel,
            CancellationToken stoppingToken)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Channel name: {channel.Name ?? "Undefined"}");
            stringBuilder.AppendLine($"Channel purpose: {channel.Purpose?.Value ?? "Undefined"}");
            stringBuilder.AppendLine($"Channel topic: {channel.Topic?.Value ?? "Undefined"}\n");

            if (messages.Any())
            {
                foreach (var message in messages)
                {
                    var user = await usersCache.GetUserAsync(message.User, stoppingToken);

                    stringBuilder.AppendLine($"From: {user.RealName} - {user.Profile.Email}");
                    var messageText = await ReplaceAllUserIdsWithUserNames(message.Text, usersCache, stoppingToken);
                    stringBuilder.AppendLine($"{DateTimeOffset.FromUnixTimeSeconds(long.Parse(message.Ts.Split(".")[0]))}: {messageText}\n");
                }
            }
            else
            {
                stringBuilder.AppendLine($"No new messages from {from} to {to}");
            }

            return stringBuilder.ToString();
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
