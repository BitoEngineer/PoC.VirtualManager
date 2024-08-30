using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PoC.VirtualManager.Interactions.Slack.Client.Models.Nested;

namespace PoC.VirtualManager.Interactions.Slack.Client.Models
{
    public class JoinConversationResponse
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        [JsonPropertyName("channel")]
        public Channel Channel { get; set; }

        [JsonPropertyName("warning")]
        public string Warning { get; set; }

        [JsonPropertyName("response_metadata")]
        public ResponseMetadata ResponseMetadata { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
}
