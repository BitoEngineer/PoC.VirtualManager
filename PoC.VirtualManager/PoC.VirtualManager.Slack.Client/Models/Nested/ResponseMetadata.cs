using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Slack.Client.Models.Nested
{

    public class ResponseMetadata
    {
        [JsonPropertyName("warnings")]
        public string[] Warnings { get; set; }
    }
}
