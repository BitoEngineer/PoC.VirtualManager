using Microsoft.SemanticKernel;
using PoC.VirtualManager.Company.Client.Models;
using PoC.VirtualManager.Company.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace PoC.VirtualManager.Company.Plugins
{
    public class CompanyKernelPlugin
    {
        private readonly ITeamsApiClient _teamsApiClient;

        public CompanyKernelPlugin(ITeamsApiClient teamsApiClient)
        {
            ArgumentNullException.ThrowIfNull(teamsApiClient, nameof(teamsApiClient));

            _teamsApiClient = teamsApiClient;
        }

        [KernelFunction("get_company_info")]
        [Description("Based on the company id obtains all the company related info")]
        [return: Description("The company info")]
        public async Task<Trait> GetCompanyInfo(int companyId)
        {
            throw new NotImplementedException();
        }
    }
}
