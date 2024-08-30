using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Models
{
    public class ChatInteractions : IInteraction
    {
        public string Id { get; set; }
        public string ChatName { get; set; }
        public Interaction[] Interactions { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public string BuildPrompt()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
