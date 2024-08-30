using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.SentimentAnalysis.Processor.Models
{
    public class InteractionMetadata
    {
        public string Id { get; set; }
        public string InteractionId { get; set; }
        public string TeamMemberId { get; set; }

        //Hard skills
        public double? TechnicalScore { get;set; } //must consider seniority
        
        //Soft skills
        public double SentimentScore { get; set; }
        public string Emotion { get; set; }
        public string Assertiveness { get; set; }
        public string Collaboration { get; set; }
        public string Conflict { get; set; }
        public double ResponseTime { get; set; }

    }
}
