using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PoC.VirtualManager.FineTuning.Infrastructure.Models
{
    public class DataSetFileMetadata
    {
        [JsonPropertyName("is_ready")]
        public bool IsReady { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("name")]
        public string FilePath { get; set; }

    }
}
