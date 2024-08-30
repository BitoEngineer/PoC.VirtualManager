using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Slack.Client.Models.Nested
{
    public class Topic
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("creator")]
        public string Creator { get; set; }

        [JsonPropertyName("last_set")]
        public long LastSet { get; set; }
    }
}
