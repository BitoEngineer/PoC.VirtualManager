using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.FineTuning
{
    public class BuildDataSetBackgroundService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //1. Iterate IDataSetBuilder
            //2. Check if should run using DataSetControl
            //3. If so, run and store DataSet using DataSetRepository
            throw new NotImplementedException();
        }
    }
}
