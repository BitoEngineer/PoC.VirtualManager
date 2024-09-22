using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PoC.VirtualManager.Utils.MongoDb.Models;

namespace PoC.VirtualManager.Company.Client.Models
{
    public class TeamMember : MongoDbEntity
    {
        public string? TeamId { get; set; }
        public string? LineManagerId { get; set; }
        public string? TechLeadId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime JoinDate { get; set; }
        public string Seniority { get; set; }
        public PersonalityTraitsScore PersonalityTraitsScore { get; set; }
        public TeamMemberBackground Background { get; set; }
    }
}
