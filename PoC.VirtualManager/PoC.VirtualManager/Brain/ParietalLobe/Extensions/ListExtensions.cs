using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.Brain.ParietalLobe.Extensions
{
    public static class ListExtensions
    {

        public static List<List<T>> SplitIntoChunks<T>(this IEnumerable<T> list, int chunkSize)
        {
            return list
                .Select((value, index) => new { value, index })
                .GroupBy(x => x.index / chunkSize)
                .Select(group => group.Select(x => x.value).ToList())
                .ToList();
        }
    }
}
