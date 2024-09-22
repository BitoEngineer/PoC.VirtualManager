using Microsoft.SemanticKernel;
using PoC.VirtualManager.Company.Client;
using PoC.VirtualManager.Company.Client.Models;
using System.ComponentModel;
using System.Reflection;

namespace PoC.VirtualManager.Plugins
{
    public class PersonalityKernelPlugin
    {
        private readonly ITeamsApiClient _teamsApiClient;

        public PersonalityKernelPlugin(ITeamsApiClient teamsApiClient)
        {
            ArgumentNullException.ThrowIfNull(teamsApiClient, nameof(teamsApiClient));

            _teamsApiClient = teamsApiClient;
        }

        [KernelFunction("get_personality_trait_meaning")]
        [Description("Gets the meaning of a specific personality trait")]
        [return: Description("The personality trait meaning")]
        public async Task<Trait> GetPersonalityTraitMeaning(string personalityTrait)
        {
            return await _teamsApiClient.GetPersonalityTraitMeaningAsync(personalityTrait);
        }
    }
}
