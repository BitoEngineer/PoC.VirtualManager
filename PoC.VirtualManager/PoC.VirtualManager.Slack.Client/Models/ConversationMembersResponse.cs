using PoC.VirtualManager.Interactions.Slack.Client.Models.Nested;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Slack.Client.Models
{
    public class ConversationMembersResponse
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        [JsonPropertyName("members")]
        public List<string> Members { get; set; }

        [JsonPropertyName("response_metadata")]
        public ResponseMetadata ResponseMetadata { get; set; }
    }
}
