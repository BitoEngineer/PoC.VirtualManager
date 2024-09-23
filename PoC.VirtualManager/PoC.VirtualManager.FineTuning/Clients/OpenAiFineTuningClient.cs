using PoC.VirtualManager.FineTuning.Clients.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PoC.VirtualManager.FineTuning.Clients
{
    public interface IOpenAiFineTuningClient 
    {
        Task<ListFineTuningJobs> ListFineTuningJobsAsync();

        Task<FileUploadResponse> UploadFineTuningFileAsync(string filePath, string fileVersion);

        Task<FineTuningJobResponse> CreateFineTuningJobAsync(
            string model,
            string trainingFileId,
            string suffix,
            string? validationFileId = null);

        Task<FineTuningJobDetail> GetFineTuningJobDetailAsync(string jobId);
    }

    public class OpenAiFineTuningClient : IOpenAiFineTuningClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _openAiApiKey;
        private const string FineTuningApiUrl = "https://api.openai.com/v1/fine_tuning/jobs";
        private const string FileUploadApiUrl = "https://api.openai.com/v1/files";

        public OpenAiFineTuningClient(IHttpClientFactory httpClientFactory, string openAiApiKey)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _openAiApiKey = openAiApiKey ?? throw new ArgumentNullException(nameof(openAiApiKey));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">One of - https://platform.openai.com/docs/guides/fine-tuning/which-models-can-be-fine-tuned</param>
        /// <param name="trainingFileId"></param>
        /// <param name="suffix"></param>
        /// <param name="validationFileId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<FineTuningJobResponse> CreateFineTuningJobAsync(
            string model, 
            string trainingFileId, 
            string suffix, 
            string? validationFileId = null)
        {
            if (string.IsNullOrEmpty(model)) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrEmpty(trainingFileId)) throw new ArgumentNullException(nameof(trainingFileId));
            if (string.IsNullOrEmpty(suffix)) throw new ArgumentNullException(nameof(suffix));

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

        public async Task<ListFineTuningJobs> ListFineTuningJobsAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiApiKey}");

            var response = await client.GetAsync(FineTuningApiUrl);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenAI API error: {response.StatusCode}, {errorContent}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var fineTuningJobsList = JsonSerializer.Deserialize<ListFineTuningJobs>(responseBody);

            return fineTuningJobsList;
        }

        public async Task<FileUploadResponse> UploadFineTuningFileAsync(string filePath, string fileVersion)
        {
            if (!File.Exists(filePath)) 
                throw new FileNotFoundException("The file does not exist.", filePath);

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);

            using var formContent = new MultipartFormDataContent();
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var fileContent = new StreamContent(fileStream);

            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            formContent.Add(new StringContent("fine-tune"), "purpose");
            formContent.Add(fileContent, "file", GetFileNameWithVersion(Path.GetFileName(filePath), fileVersion));

            var response = await client.PostAsync(FileUploadApiUrl, formContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenAI API error: {response.StatusCode}, {errorContent}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var uploadResponse = JsonSerializer.Deserialize<FileUploadResponse>(responseBody);

            return uploadResponse;
        }

        public async Task<FineTuningJobDetail> GetFineTuningJobDetailAsync(string jobId)
        {
            if (string.IsNullOrEmpty(jobId)) throw new ArgumentNullException(nameof(jobId));

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiApiKey}");

            var response = await client.GetAsync($"https://api.openai.com/v1/fine_tuning/jobs/{jobId}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"OpenAI API error: {response.StatusCode}, {errorContent}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var fineTuningJobDetail = JsonSerializer.Deserialize<FineTuningJobDetail>(responseBody);

            return fineTuningJobDetail;
        }

        private static string GetFileNameWithVersion(string fileName, string version)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string fileExtension = Path.GetExtension(fileName);
            return $"{fileNameWithoutExtension}_{version.Replace(".", "_")}{fileExtension}";
        }
    }
}
