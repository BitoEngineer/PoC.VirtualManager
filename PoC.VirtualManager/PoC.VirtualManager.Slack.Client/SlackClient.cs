using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using PoC.VirtualManager.Interactions.Slack.Client.Models;
using System.Net.Http;
using PoC.VirtualManager.Slack.Client.Models;

namespace PoC.VirtualManager.Slack.Client
{
    public interface ISlackClient
    {
        Task<bool> SendMessageAsync(string channelId, string message, CancellationToken cancellationToken);
        Task<ListConversationsResponse> ListAllConversationsAsync(CancellationToken cancellationToken);
        Task<ListUsersConversationsResponse> ListUsersConversationsAsync(CancellationToken cancellationToken);
        Task<ListUsersResponse> ListUsersAsync(CancellationToken cancellationToken);
        Task<string> GetConversationInfoAsync(string channelId, CancellationToken cancellationToken);
        Task<string> GetConversationMembersAsync(string channelId, CancellationToken cancellationToken);
        Task<ConversationsHistoryResponse> GetConversationHistoryAsync(string conversationId,
            DateTimeOffset from,
            DateTimeOffset to,
            CancellationToken cancellationToken);
        Task<JoinConversationResponse> JoinConversationAsync(string channelId, CancellationToken cancellationToken);
    }

    public class SlackClient : ISlackClient
    {
        private const string URL = "https://slack.com/api/";

        private readonly SlackSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;

        public SlackClient(SlackSettings settings, IHttpClientFactory httpClientFactory)
        {
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));
            ArgumentNullException.ThrowIfNull(httpClientFactory, nameof(httpClientFactory));

            _settings = settings;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> SendMessageAsync(string channelId, string message, CancellationToken cancellationToken)
        {
            HttpClient client = GetAuthenticatedClient();
            var content = new StringContent(JsonSerializer.Serialize(new
            {
                channel = channelId,
                text = message
            }), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(URL + "chat.postMessage", content, cancellationToken);

            response.EnsureSuccessStatusCode();

            var responseBodyJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseBody = JsonSerializer.Deserialize<SlackMessageResponse>(responseBodyJson);

            return responseBody.Ok;
        }

        public async Task<ListConversationsResponse> ListAllConversationsAsync(CancellationToken cancellationToken)
        {
            var client = GetAuthenticatedClient();
            var queryParams = "?types=public_channel,private_channel,mpim,im&limit=999";
            var response = await client.GetAsync(URL + "conversations.list" + queryParams, cancellationToken);

            response.EnsureSuccessStatusCode();

            var responseBodyJson = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<ListConversationsResponse>(responseBodyJson);
        }

        public async Task<string> GetConversationInfoAsync(string channelId, CancellationToken cancellationToken)
        {
            var client = GetAuthenticatedClient();
            var queryParams = $"?channel={channelId}&include_num_members=true";
            var response = await client.GetAsync(URL + "conversations.info" + queryParams, cancellationToken);

            response.EnsureSuccessStatusCode();

            var responseBodyJson = await response.Content.ReadAsStringAsync(cancellationToken);
            return responseBodyJson;
            //return JsonSerializer.Deserialize<ListChannelsResponse>(responseBodyJson);
        }

        public async Task<string> GetConversationMembersAsync(string channelId, CancellationToken cancellationToken)
        {
            var client = GetAuthenticatedClient();
            var queryParams = $"?channel={channelId}";
            var response = await client.GetAsync(URL + "conversations.members\r\n" + queryParams, cancellationToken);

            response.EnsureSuccessStatusCode();

            var responseBodyJson = await response.Content.ReadAsStringAsync(cancellationToken);
            return responseBodyJson;
            //return JsonSerializer.Deserialize<ListChannelsResponse>(responseBodyJson);
        }

        public async Task<ListUsersConversationsResponse> ListUsersConversationsAsync(CancellationToken cancellationToken)
        {
            var client = GetAuthenticatedClient();
            var queryParams = "?types=public_channel,private_channel,mpim,im&limit=999";
            var response = await client.GetAsync(URL + "users.conversations" + queryParams, cancellationToken);

            response.EnsureSuccessStatusCode();

            var responseBodyJson = await response.Content.ReadAsStringAsync(cancellationToken);

            //TODO pagination

            return JsonSerializer.Deserialize<ListUsersConversationsResponse>(responseBodyJson);
        }

        public async Task<ListUsersResponse> ListUsersAsync(CancellationToken cancellationToken)
        {
            var client = GetAuthenticatedClient();
            var response = await client.GetAsync(URL + "users.list", cancellationToken);

            response.EnsureSuccessStatusCode();

            var responseBodyJson = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<ListUsersResponse>(responseBodyJson);
        }

        public async Task<ConversationsHistoryResponse> GetConversationHistoryAsync(string conversationId,
            DateTimeOffset from,
            DateTimeOffset to,
            CancellationToken cancellationToken)
        {
            var client = GetAuthenticatedClient();
            var queryParams = $"?channel={conversationId}&oldest={from.ToUnixTimeSeconds()}&latest={to.ToUnixTimeSeconds()}&limit=999&inclusive=true";
            var response = await client.GetAsync(URL + "conversations.history" + queryParams, cancellationToken);

            response.EnsureSuccessStatusCode();

            var responseBodyJson = await response.Content.ReadAsStringAsync(cancellationToken);

            //TODO pagination

            return JsonSerializer.Deserialize<ConversationsHistoryResponse>(responseBodyJson);
        }

        public async Task<JoinConversationResponse> JoinConversationAsync(string channelId, CancellationToken cancellationToken)
        {
            var client = GetAuthenticatedClient();
            var content = new StringContent(JsonSerializer.Serialize(new
            {
                channel = channelId,
            }), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(URL + "conversations.join", content, cancellationToken);

            response.EnsureSuccessStatusCode();

            var responseBodyJson = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<JoinConversationResponse>(responseBodyJson);
        }

        private HttpClient GetAuthenticatedClient()
        {
            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.AccessToken);
            return client;
        }
    }
}
