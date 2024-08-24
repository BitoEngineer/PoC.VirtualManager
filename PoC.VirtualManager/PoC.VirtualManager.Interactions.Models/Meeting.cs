using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Models
{
    public class Meeting : IInteraction
    {
        public string MeetingTitle { get; set; }
        public string MeetingDescription { get; set; }
        public Interaction[] Transcript { get; set; }

        public string BuildPrompt()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
