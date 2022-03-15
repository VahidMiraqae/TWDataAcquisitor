using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWCommon
{
    public static class CandleReader
    {
        public static Candle[] ReadCandles(string filename)
        {
            if (!File.Exists(filename))
            {
                return null;
            }

            var lines = File.ReadLines(filename);

            return lines.Select(aa => Candle.Parse(aa)).Reverse().ToArray();
        }
    }
}
