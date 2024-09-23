using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PoC.VirtualManager.FineTuning.Clients;
using PoC.VirtualManager.FineTuning.Infrastructure;
using PoC.VirtualManager.FineTuning.Infrastructure.Models;
using PoC.VirtualManager.FineTuning.Utils;

namespace PoC.VirtualManager.FineTuning
{
    public class FinetuningBackgroundService : BackgroundService
    {
        private readonly IDataSetRepository _dataSetRepository;
        private readonly IDataSetControlRepository _dataSetControlRepository;
        private readonly ILogger<FinetuningBackgroundService> _logger;
        private readonly IOpenAiFineTuningClient _fineTuningClient;

        public FinetuningBackgroundService(IDataSetRepository dataSetRepository,
            IDataSetControlRepository dataSetControlRepository,
            ILogger<FinetuningBackgroundService> logger,
            IOpenAiFineTuningClient fineTuningClient)
        {
            ArgumentNullException.ThrowIfNull(dataSetRepository, nameof(dataSetRepository));
            ArgumentNullException.ThrowIfNull(dataSetControlRepository, nameof(dataSetControlRepository));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            ArgumentNullException.ThrowIfNull(fineTuningClient, nameof(fineTuningClient));

            _dataSetRepository = dataSetRepository;
            _dataSetControlRepository = dataSetControlRepository;
            _logger = logger;
            _fineTuningClient = fineTuningClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //TODO list fine tuning jobs - get https://api.openai.com/v1/fine_tuning/jobs
            await foreach (var dataSetBundle in _dataSetRepository.GetAllDataSetAsync(stoppingToken))
            {
                if (!dataSetBundle.IsReady)
                {
                    _logger.LogWarning("{DataSetName} wont be processed because its not ready yet.", dataSetBundle.Name);
                    continue;
                }

                var latestScriptControl = await _dataSetControlRepository.GetLatestByNameAsync(dataSetBundle.Name, stoppingToken);

                if (latestScriptControl != null && !dataSetBundle.Version.IsBiggerThan(latestScriptControl.Version))
                {
                    _logger.LogWarning("{DataSetName} wont be processed because version {ScriptVersion} is lower than the last run {LastRunVersion}.", 
                        dataSetBundle.Name,
                        dataSetBundle.Version,
                        latestScriptControl.Version);

                    continue;
                }

                //TODO upload file - https://platform.openai.com/docs/api-reference/files/create
                //TODO run dataset
                //_fineTuningClient.CreateFineTuningJobAsync()

                await _dataSetControlRepository.InsertAsync(new DataSetControl
                {
                    ScriptName = dataSetBundle.Name,
                    Version = dataSetBundle.Version,
                    //TODO FineTuningJobId
                }, stoppingToken);
            }
        }

        //TODO override Stop and cancel on going fine tuning job
    }
}
