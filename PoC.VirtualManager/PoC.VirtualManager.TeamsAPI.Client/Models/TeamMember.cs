using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Teams.Client.Models
{
    public class TeamMember
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int? LineManagerId { get; set; }
        public int? TechLeadId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public DateTime BirthDate { get; set; }
        public int YearsOfExperience { get; set; }
        public PersonalityTraitsScore PersonalityTraitsScore { get; set; }
        public TeamMemberBackground Background { get; set; }
    }
}
