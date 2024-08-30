using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Slack.Client.Models
{
    public class ListUsersConversationsResponse
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        [JsonPropertyName("channels")]
        public List<UserConversation> Conversations { get; set; }

        [JsonPropertyName("response_metadata")]
        public ResponseMetadata ResponseMetadata { get; set; }

    }

    public class UserConversation
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("is_archived")]
        public bool IsArchived { get; set; }

        [JsonPropertyName("is_im")]
        public bool IsIm { get; set; }

        [JsonPropertyName("is_org_shared")]
        public bool IsOrgShared { get; set; }

        [JsonPropertyName("context_team_id")]
        public string ContextTeamId { get; set; }

        [JsonPropertyName("updated")]
        public long Updated { get; set; }

        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("is_user_deleted")]
        public bool IsUserDeleted { get; set; }

        [JsonPropertyName("priority")]
        public int Priority { get; set; }
    }

}
