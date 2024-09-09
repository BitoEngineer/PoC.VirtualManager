using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Models
{
    internal class InteractionsSummary
    {
        public string Summary { get; set; }
        public string InteractionId { get; set; }
        public TeamMemberInteractionSummary[] ByTeamMember { get; set; }
    }

    public class TeamMemberInteractionSummary
    {
        public string TeamMemberId { get; set; }
        public double AverageResponseTime { get; set; }
        public int TotalMessages { get; set; }
        public int AverageMessageLength { get; set; }
        public string[] MetadataIds { get; set; }
    }
}
