using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Company.Client.Models
{
    public class PersonalityTraits
    {
        public Trait Openness { get; set; }
        public IntellectTrait Intellect { get; set; }
        public Trait OpennessToExperience { get; set; }
        public Trait Volatility { get; set; }
        public Trait Withdrawal { get; set; }
        public Trait Neuroticism { get; set; }
        public Trait Assertiveness { get; set; }
        public Trait Enthusiasm { get; set; }
        public Trait Extraversion { get; set; }
        public Trait Orderliness { get; set; }
        public Trait Industriousness { get; set; }
        public Trait Conscientiousness { get; set; }
        public Trait Politeness { get; set; }
        public Trait Compassion { get; set; }
        public Trait Agreeableness { get; set; }
    }

    public class Trait
    {
        public string Description { get; set; }
    }

    public class IntellectTrait : Trait
    {
        public string Note { get; set; }
    }

}
