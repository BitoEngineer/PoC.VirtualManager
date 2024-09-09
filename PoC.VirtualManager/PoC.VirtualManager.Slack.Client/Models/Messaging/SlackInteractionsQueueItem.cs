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
        public string ChannelName { get; set; }
        public string ChannelPurpose { get; set; }
        public string ChannelTopic { get; set; }
        public List<SlackMessage> Messages { get; set; }
    }
}
