using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWCommon
{
    public class ChartDataReader : ILatestChartDataReader
    {
        private IChartDataPathMaker _pathMaker;

        public ChartDataReader(IChartDataPathMaker chartDataPathMaker)
        {
            _pathMaker = chartDataPathMaker;
        }

        public Candle GetLatestData(string source, string symbol, string timeframe)
        { 
            var path = _pathMaker.GetPath(symbol, source, timeframe);

            if (!File.Exists(path))
            {
                return null;
            }

            var firstLine = File.ReadLines(path).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(firstLine) || firstLine.Length < 10)
            {
                return null;
            }

            var candle = Candle.Parse(firstLine);

            return candle;
        }
    }
}
