using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using PoC.VirtualManager.Teams.Client.Models;
using System.Text.Json.Serialization;

namespace PoC.VirtualManager.Teams.Api.Dtos
{
    public class TeamMemberDto
    {
        public string TeamId { get; set; }
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
