using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Models
{
    public class InteractionMetadata
    {
        //Manual filled fields
        [BsonElement("_id")]
        [JsonPropertyName("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TeamMemberEmail { get; set; }
        public string ChannelName { get; set; }
        public string ChannelId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        //Hard skills
        [Description("Evaluation of technical expertise shown on the interaction, considering team member seniority.")]
        public string TechnicalExpertise { get; set; } //must consider seniority

        //Soft skills
        public SentimentResult Sentiment { get; set; }

        [Description("Emotion analysis.")]
        public string Emotion { get; set; }

        [Description("Assertiveness analysis.")]
        public string Assertiveness { get; set; }

        [Description("Collaboration analysis.")]
        public string Collaboration { get; set; }

        [Description("Conflict analysis.")]
        public string Conflict { get; set; }

        //Suggestions
        [Description("TODO")]
        public string SoftSkillsSuggestions { get; set; }
        [Description("TODO")]
        public string HardSkillsSuggestions { get; set; }
    }

    public class SentimentResult
    {
        public string Opinion { get; set; }
        public string Sentiment { get; set; }

        public ConfidenceScore Score { get; set; }
    }

    public class ConfidenceScore
    {
        public double Positive { get; set; }

        public double Negative { get; set; }

        public double Neutral { get; set; }
    }
}
