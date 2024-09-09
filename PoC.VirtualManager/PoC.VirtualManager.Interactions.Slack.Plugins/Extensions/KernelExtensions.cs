using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Slack.Plugins.Extensions
{
    public static class KernelExtensions
    {
        public static IKernelBuilder BindSlackPlugins(this IKernelBuilder kernelBuilder, IServiceProvider serviceProvider)
        {
            kernelBuilder.Plugins.AddFromObject(serviceProvider.GetRequiredService<SlackKernelPlugin>(), "slack_plugin");

            return kernelBuilder;
        }
    }
}
