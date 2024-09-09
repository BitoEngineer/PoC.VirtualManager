using PoC.VirtualManager.Interactions.Slack.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Slack.Client.Models.Messaging
{
    public class SlackMessage
    {
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $@"From: {FromName} - {FromEmail}\n
                     {Timestamp}: {Message}
";
        }
    }
}
