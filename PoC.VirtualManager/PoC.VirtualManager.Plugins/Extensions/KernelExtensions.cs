using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using PoC.VirtualManager.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Teams.Plugins.Extensions
{
    public static class KernelExtensions
    {
        public static IKernelBuilder BindTeamsPlugins(this IKernelBuilder kernelBuilder, IServiceProvider serviceProvider)
        {
            kernelBuilder.Plugins.AddFromObject(serviceProvider.GetRequiredService<PersonalityKernelPlugin>(), "personality_plugin");
            kernelBuilder.Plugins.AddFromObject(serviceProvider.GetRequiredService<CompanyKernelPlugin>(), "company_plugin");
            kernelBuilder.Plugins.AddFromObject(serviceProvider.GetRequiredService<TeamKernelPlugin>(), "teams_plugin");

            return kernelBuilder;
        }
    }
}
