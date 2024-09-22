using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace PoC.VirtualManager.Company.Client.Models
{
    public class Team
    {
        [BsonElement("_id")]
        [JsonPropertyName("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
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
