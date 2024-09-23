using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PoC.VirtualManager.FineTuning.Clients.Models
{
    public class FineTuningJobResponse
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("fine_tuned_model")]
        public string FineTunedModel { get; set; }

        [JsonPropertyName("organization_id")]
        public string OrganizationId { get; set; }

        [JsonPropertyName("result_files")]
        public List<ResultFile> ResultFiles { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("validation_file")]
        public string ValidationFile { get; set; }

        [JsonPropertyName("training_file")]
        public string TrainingFile { get; set; }
    }

    public class ResultFile
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("purpose")]
        public string Purpose { get; set; }
    }

}
