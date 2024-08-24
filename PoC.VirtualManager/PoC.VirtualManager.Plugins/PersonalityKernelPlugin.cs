using Microsoft.SemanticKernel;
using PoC.VirtualManager.Teams.Client;
using PoC.VirtualManager.Teams.Client.Models;
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
            var traits = await _teamsApiClient.GetPersonalityTraitMeaningAsync(personalityTrait);
            return GetTraitByName(personalityTrait, traits);
        }

        public Trait GetTraitByName(string traitName, PersonalityTraits traits)
        {
            PropertyInfo property = typeof(PersonalityTraits).GetProperty(traitName);

            if (property == null)
            {
                throw new ArgumentException("Invalid trait name");
            }

            if (!typeof(Trait).IsAssignableFrom(property.PropertyType))
            {
                throw new ArgumentException("Property is not of type Trait");
            }

            return (Trait)property.GetValue(traits);
        }
    }
}
