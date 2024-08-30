using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Slack.Client.Models
{
    public class ConversationsHistoryResponse
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        [JsonPropertyName("latest")]
        public string Latest { get; set; }

        [JsonPropertyName("messages")]
        public List<ConversationMessage> Messages { get; set; }

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        [JsonPropertyName("pin_count")]
        public int PinCount { get; set; }

        [JsonPropertyName("response_metadata")]
        public ResponseMetadata ResponseMetadata { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
    }

    public class Attachment
    {
        [JsonPropertyName("service_name")]
        public string ServiceName { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("fallback")]
        public string Fallback { get; set; }

        [JsonPropertyName("thumb_url")]
        public string ThumbUrl { get; set; }

        [JsonPropertyName("thumb_width")]
        public int ThumbWidth { get; set; }

        [JsonPropertyName("thumb_height")]
        public int ThumbHeight { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }

    public class ConversationMessage
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("user")]
        public string User { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("ts")]
        public string Ts { get; set; }

        [JsonPropertyName("attachments")]
        public List<Attachment> Attachments { get; set; }
    }
}
