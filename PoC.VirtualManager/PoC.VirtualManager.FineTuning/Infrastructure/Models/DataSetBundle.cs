using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PoC.VirtualManager.FineTuning.Infrastructure.Models
{
    public class DataSetBundle
    {
        [JsonPropertyName("is_ready")]
        public bool IsReady { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("dataset")]
        public List<DataSet> DataSet { get; set; }
    }

    public class DataSet
    {
        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public RoleEnum Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public enum RoleEnum
    {
        System,
        Assistant,
        User
    }
}
