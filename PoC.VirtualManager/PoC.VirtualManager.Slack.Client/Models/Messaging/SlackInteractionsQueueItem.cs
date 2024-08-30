using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Slack.Client.Models.Messaging
{
    public class SlackInteractionsQueueItem
    {
        public string ChannelId { get; set; }
        public string Message { get; set; }
    }
}
