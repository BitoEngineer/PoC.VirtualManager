using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.FineTuning.Utils
{
    internal static class VersionComparer
    {
        public static bool IsBiggerThan(this string version1, string version2)
        {
            Version v1 = new Version(version1);
            Version v2 = new Version(version2);

            return v2 > v1;
        }
    }
}
