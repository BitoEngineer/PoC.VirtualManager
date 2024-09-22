using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PoC.VirtualManager.Utils.MongoDb.Models;
using System.Text.Json.Serialization;

namespace PoC.VirtualManager.Company.Client.Models
{
    public class Team : MongoDbEntity
    {
        public string Name { get; set; }
        public List<string> TeamMembersIds { get; set; } = new List<string>();
        public string Description { get; set; }
        public string Department { get; set; }
        public string DomainExpertise { get; set; }
        public string TechnicalExpertise { get; set; }
        public string Methodoly { get; set; }

        //TODO add related entity with the Team goals and roadmap
    }
}
