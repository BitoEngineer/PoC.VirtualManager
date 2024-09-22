using PoC.VirtualManager.Interactions.Slack.Listener.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Models
{
    public class VirtualManagerSettings
    {
        public SlackListenerSettings SlackListenerSettings { get; set; }
    }
}
