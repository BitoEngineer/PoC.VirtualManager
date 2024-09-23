using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PoC.VirtualManager.FineTuning.Clients;
using PoC.VirtualManager.FineTuning.Clients.Models;
using PoC.VirtualManager.FineTuning.Infrastructure;
using PoC.VirtualManager.FineTuning.Infrastructure.Models;
using PoC.VirtualManager.FineTuning.Utils;
using ZstdSharp.Unsafe;

namespace PoC.VirtualManager.FineTuning
{
    public class FinetuningBackgroundService : BackgroundService
    {
        private readonly IDataSetRepository _dataSetRepository;
        private readonly IDataSetControlRepository _dataSetControlRepository;
        private readonly ILogger<FinetuningBackgroundService> _logger;
        private readonly IOpenAiFineTuningClient _fineTuningClient;
        private readonly Settings _settings;

        public FinetuningBackgroundService(IDataSetRepository dataSetRepository,
            IDataSetControlRepository dataSetControlRepository,
            ILogger<FinetuningBackgroundService> logger,
            IOpenAiFineTuningClient fineTuningClient,
            Settings settings)
        {
            ArgumentNullException.ThrowIfNull(dataSetRepository, nameof(dataSetRepository));
            ArgumentNullException.ThrowIfNull(dataSetControlRepository, nameof(dataSetControlRepository));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ArgumentNullException.ThrowIfNull(fineTuningClient, nameof(fineTuningClient));
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));

            _dataSetRepository = dataSetRepository;
            _dataSetControlRepository = dataSetControlRepository;
            _logger = logger;
            _fineTuningClient = fineTuningClient;
            _settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var jobsList = await _fineTuningClient.ListFineTuningJobsAsync();
            _logger.LogInformation("Jobs history: "+string.Join("\n", jobsList.Data.Select(j => $"{ j.CreatedAtDate}: {j.Id} - {j.Message} ; {j.Object}")));

            await foreach (var dataSetFileMetadata in _dataSetRepository.GetAllDataSetFilesMetadataAsync(stoppingToken))
            {
                var filename = Path.GetFileName(dataSetFileMetadata.FilePath);
                if (!dataSetFileMetadata.IsReady)
                {
                    _logger.LogWarning("{DataSetName} wont be processed because its not ready yet.", filename);
                    continue;
                }

                var latestScriptControl = await _dataSetControlRepository.GetLatestByNameAsync(filename, stoppingToken);

                if (latestScriptControl != null && !dataSetFileMetadata.Version.IsBiggerThan(latestScriptControl.Version))
                {
                    _logger.LogWarning("{DataSetName} wont be processed because version {ScriptVersion} is lower than the last run {LastRunVersion}.",
                        filename,
                        dataSetFileMetadata.Version,
                        latestScriptControl.Version);

                    continue;
                }

                await _dataSetControlRepository.InsertAsync(new DataSetControl
                {
                    FileName = Path.GetFileName(dataSetFileMetadata.FilePath),
                    Version = dataSetFileMetadata.Version,
                    Status = "WillBeProcessed",
                }, stoppingToken);

                var details = await ProcessFileTuningFile(dataSetFileMetadata, filename);

                await _dataSetControlRepository.InsertAsync(new DataSetControl
                {
                    FileName = Path.GetFileName(dataSetFileMetadata.FilePath),
                    Version = dataSetFileMetadata.Version,
                    OpenAiFileId = details.TrainingFile,
                    OpenAiJobId = details.Id,
                    Status = details.Status,
                }, stoppingToken);
            }
        }

        private async Task<FineTuningJobDetail> ProcessFileTuningFile(DataSetFileMetadata dataSetFileMetadata, string filename)
        {
            var uploadFileResponse = await _fineTuningClient.UploadFineTuningFileAsync(dataSetFileMetadata.FilePath, dataSetFileMetadata.Version);
            var fineTuningJob = await _fineTuningClient.CreateFineTuningJobAsync(
                model: _settings.OpenAiModel,
                suffix: _settings.OpenAiModelSuffix,
                trainingFileId: uploadFileResponse.Id);

            _logger.LogInformation("{DataSetName} fine tuning job started - {JobId}.", filename, fineTuningJob.Id);

            FineTuningJobDetail jobDetails;
            while (!(jobDetails = await _fineTuningClient.GetFineTuningJobDetailAsync(fineTuningJob.Id))
                        .HasFinished())
            {
                _logger.LogInformation("Job {JobId} stills running. Status is {JobStatus}.", jobDetails.Id, jobDetails.Status);

                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            if (jobDetails.Status == "succeeded")
            {
                _logger.LogInformation("Job {JobId} ran with success.", jobDetails.Id);
            }
            else
            {
                _logger.LogWarning("Job {JobId} failed for some reason. Status is {JobStatus}.", jobDetails.Id, jobDetails.Status);
            }

            return jobDetails;
        }
    }
}
