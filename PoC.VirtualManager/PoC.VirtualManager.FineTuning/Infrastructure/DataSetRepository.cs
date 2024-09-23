using PoC.VirtualManager.FineTuning.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.FineTuning.Infrastructure
{
    public interface IDataSetRepository
    {
        IAsyncEnumerable<DataSetFileMetadata> GetAllDataSetFilesMetadataAsync(CancellationToken stoppingToken);
    }

    internal class DataSetRepository
    {
        //TODO abstract File.ReadAllText
    }
}
