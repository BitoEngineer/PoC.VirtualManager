using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PoC.VirtualManager.Utils.MongoDb.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Models
{
    public class InteractionMetadata : MongoDbEntity
    {
        public string TeamMemberEmail { get; set; }
        public string ChannelName { get; set; }
        public string ChannelId { get; set; }
        public string Text { get; set; }
        public InteractionSource Source { get; set; }

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
        [Description("Suggestions on soft skills. May be null if none.")]
        public string SoftSkillsSuggestions { get; set; }

        [Description("Suggestions on hard skills (considering seniority level). May be null if none.")]
        public string HardSkillsSuggestions { get; set; }

        [Description("Practical suggestions on how to handle the issue in a better way. May be null if none.")]
        public string PracticalSuggestions { get; set; }

        //Actions
        //TODO - should be communicated to
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
