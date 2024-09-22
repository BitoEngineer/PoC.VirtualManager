using PoC.VirtualManager.Interactions.Slack.Client;
using PoC.VirtualManager.Interactions.Slack.Client.Models;
using System.Text.Json;

namespace PoC.VirtualManager.Playground.Apollo
{
    internal class ApolloSlackClient : ISlackClient
    {
        public static Dictionary<int, string> InteractionsFiles;

        static ApolloSlackClient()
        {
            InteractionsFiles = new Dictionary<int, string>(GetFilesDictionary());
        }

        public ApolloSlackClient()
        {
        }

        public async Task<ConversationsHistoryResponse> GetConversationHistoryAsync(
            string conversationId,
            DateTimeOffset from,
            DateTimeOffset to,
            CancellationToken cancellationToken)
        {
            PrintInteractionsOptions();
            int interactionOption = int.Parse(Console.ReadLine());
            return JsonSerializer.Deserialize<ConversationsHistoryResponse>(
                await File.ReadAllTextAsync(InteractionsFiles[interactionOption])
            );
        }

        public Task<string> GetConversationInfoAsync(
            string channelId,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetConversationMembersAsync(
            string channelId,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<JoinConversationResponse> JoinConversationAsync(
            string channelId,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ListConversationsResponse> ListAllConversationsAsync(CancellationToken cancellationToken)
        {
            return JsonSerializer.Deserialize<ListConversationsResponse>(await File.ReadAllTextAsync("./Apollo/Slack/Apollo_Team_Channel.json"));
        }

        public async Task<ListUsersResponse> ListUsersAsync(CancellationToken cancellationToken)
        {
            return JsonSerializer.Deserialize<ListUsersResponse>(await File.ReadAllTextAsync("./Apollo/Slack/Apollo_Slack_Users.json"));
        }

        public Task<ListUsersConversationsResponse> ListUsersConversationsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendMessageAsync(string channelId, string message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public static Dictionary<int, string> GetFilesDictionary()
        {
            Dictionary<int, string> filePaths = new Dictionary<int, string>();
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Apollo/Slack/Interactions", "*.*", SearchOption.AllDirectories);

            int index = 1;
            foreach (string file in files)
            {
                filePaths.Add(index, file);
                index++;
            }

            return filePaths;
        }

        public static void PrintInteractionsOptions()
        {
            foreach (var entry in InteractionsFiles)
            {
                Console.WriteLine($"{entry.Key}: {Path.GetFileName(entry.Value)}");
            }
        }
    }
}
