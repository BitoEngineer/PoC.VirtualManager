using PoC.VirtualManager.FineTuning.Clients.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PoC.VirtualManager.FineTuning.Clients
{
    public interface IOpenAiFineTuningClient 
    {
        Task<FineTuningJobResponse> CreateFineTuningJobAsync(
            string model,
            string trainingFileId,
            string? suffix = null,
            string? validationFileId = null);
    }

    public class OpenAiFineTuningClient : IOpenAiFineTuningClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _openAiApiKey;
        private const string FineTuningApiUrl = "https://api.openai.com/v1/fine_tuning/jobs";

        public OpenAiFineTuningClient(IHttpClientFactory httpClientFactory, string openAiApiKey)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _openAiApiKey = openAiApiKey ?? throw new ArgumentNullException(nameof(openAiApiKey));
        }

        public async Task<FineTuningJobResponse> CreateFineTuningJobAsync(
            string model, 
            string trainingFileId, 
            string? suffix = null, 
            string? validationFileId = null)
        {
            if (string.IsNullOrEmpty(model)) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrEmpty(trainingFileId)) throw new ArgumentNullException(nameof(trainingFileId));

            var requestBody = new
            {
                model,
                training_file = trainingFileId,
                suffix,
                validation_file = validationFileId
            };

            var jsonRequestBody = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiApiKey}");

            var response = await client.PostAsync(FineTuningApiUrl, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenAI API error: {response.StatusCode}, {errorContent}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FineTuningJobResponse>(responseBody);
        }
    }
}
