using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Teams.Client.Models
{
    public class TeamMember
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public int Age { get; set; }
        public string Background { get; set; }
        public int YearsOfExperience { get; set; }
        public PersonalityTraitsScore PersonalityTraitsScore { get; set; }
    }
}
