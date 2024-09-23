using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PoC.VirtualManager.FineTuning.Clients.Models
{
    public class ListFineTuningJobs
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("data")]
        public List<FineTuningJobEvent> Data { get; set; }

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }

    public class FineTuningJobEvent
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }
        public DateTime CreatedAtDate => DateTimeOffset.FromUnixTimeSeconds(CreatedAt).DateTime;

        [JsonPropertyName("level")]
        public string Level { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public object Data { get; set; } // This can be refined based on the actual structure of `data`

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
