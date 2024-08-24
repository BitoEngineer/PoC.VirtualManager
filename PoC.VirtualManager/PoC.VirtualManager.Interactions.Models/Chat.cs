using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Models
{
    public class Chat : IInteraction
    {
        public string ChatName { get; set; }
        public Interaction[] Interactions { get; set; }

        public string BuildPrompt()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
