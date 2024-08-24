using PoC.VirtualManager.Personality.Leadership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Personality
{
    public class Personality
    {
        public LeadershipStyle LeadershipStyle { get; }

        public Personality(LeadershipStyle leadershipStyle)
        {
            LeadershipStyle = leadershipStyle;
        }
    }
}
