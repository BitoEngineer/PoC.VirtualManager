using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PoC.VirtualManager.FineTuning.Infrastructure.Models
{
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
