using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Interactions.Models
{
    public interface IInteraction
    {
        string BuildPrompt();
    }
}
