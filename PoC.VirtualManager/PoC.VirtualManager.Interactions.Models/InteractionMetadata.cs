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
        [Description("Sentiment analysis result.")]
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
        [Description("Overall evaluation of the sentiment shown in the interaction.")]
        public string Sentiment { get; set; }

        [Description("Confidence score based on the sentiment analysis result.")]
        public ConfidenceScore Score { get; set; }
    }

    public class ConfidenceScore
    {
        [Description("Positive sentiment analysis score.")]
        public double Positive { get; set; }

        [Description("Negative sentiment analysis score.")]
        public double Negative { get; set; }

        [Description("Neutral sentiment analysis score.")]
        public double Neutral { get; set; }
    }
}
