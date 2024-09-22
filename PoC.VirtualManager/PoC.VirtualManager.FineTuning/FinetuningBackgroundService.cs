using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.FineTuning
{
    public class FinetuningBackgroundService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //1. Iterate datasets that are ready and didnt run yet
            //2. Run dataset
            //3. Update dataset using DataSetControl
            throw new NotImplementedException();
        }
    }
}
