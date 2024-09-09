using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Brain.TemporalLobe
{
    internal class InteractionsSummarizerBackgroundService : BackgroundService
    {
        //Periodically summarizes batches of interactions
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //throw new NotImplementedException();
            return Task.CompletedTask;
        }
    }
}
