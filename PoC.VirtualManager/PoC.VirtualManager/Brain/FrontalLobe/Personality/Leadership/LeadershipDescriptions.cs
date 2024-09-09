using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Brain.FrontalLobe.Personality.Leadership
{
    public class LeadershipDescriptions
    {
        public static Dictionary<LeadershipStyle, string> LeadershipStyleDescriptions = new Dictionary<LeadershipStyle, string>
        {
            { LeadershipStyle.Transformational,
              "Transformational leaders inspire and motivate their team by creating a compelling vision of the future, fostering an environment of trust, and encouraging innovation and creativity. They focus on the development and well-being of their team members, aiming to transform both the team and the organization for the better. " +
              "Book Reference: 'Leaders Eat Last' by Simon Sinek. This book explores how leaders can build trust and cooperation within their teams, making it a strong example of transformational leadership."
            },

            { LeadershipStyle.Servant,
              "Servant leaders prioritize the needs of their team members and the organization above their own. They lead by serving others, focusing on the growth, development, and well-being of their followers. Servant leaders ensure that their team has the resources and support needed to succeed, creating a positive and empowering environment. " +
              "Book Reference: 'The Servant: A Simple Story About the True Essence of Leadership' by James C. Hunter. This book provides a deep dive into the principles of servant leadership, emphasizing the importance of serving others."
            },

            { LeadershipStyle.Autocratic,
              "Autocratic leaders make decisions unilaterally, with minimal input from team members. They provide clear instructions and expect followers to comply with their directives. This style is often effective in situations requiring quick decision-making or where the leader possesses the most expertise. However, it can sometimes lead to lower morale and engagement among team members. " +
              "Book Reference: 'The One Minute Manager' by Ken Blanchard and Spencer Johnson. While this book is more focused on effective management, it provides insights into when a more directive, autocratic leadership style can be effective."
            }
        };
    }
}
