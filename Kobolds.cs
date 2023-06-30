using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiamet2._0
{
    public static class Kobolds
    {
        public static IEnumerable<string> SplitIntoChunks(string input, int chunkSize = 2000)
        {
            for (int i = 0; i < input.Length; i += chunkSize)
            {
                yield return input.Substring(i, Math.Min(chunkSize, input.Length - i));
            }
        }
    }
}
