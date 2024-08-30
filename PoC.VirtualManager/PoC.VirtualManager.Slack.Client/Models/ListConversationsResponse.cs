using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PoC.VirtualManager.Interactions.Slack.Client.Models.Nested;

namespace PoC.VirtualManager.Interactions.Slack.Client.Models
{
    public class ListConversationsResponse
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        [JsonPropertyName("channels")]
        public List<Channel> Conversations { get; set; }

        [JsonPropertyName("response_metadata")]
        public ResponseMetadata ResponseMetadata { get; set; }
    }

    public class Properties
    {
        [JsonPropertyName("canvas")]
        public Canvas Canvas { get; set; }

        [JsonPropertyName("use_case")]
        public string UseCase { get; set; }
    }

    public class Canvas
    {
        [JsonPropertyName("file_id")]
        public string FileId { get; set; }

        [JsonPropertyName("is_empty")]
        public bool IsEmpty { get; set; }

        [JsonPropertyName("quip_thread_id")]
        public string QuipThreadId { get; set; }
    }

}
