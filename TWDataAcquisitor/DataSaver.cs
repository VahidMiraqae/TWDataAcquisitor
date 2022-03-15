using System.Collections.Generic;
using System.IO;
using System.Linq;
using TWCommon;

namespace TWDataAcquisitor
{
    internal class DataSaver : IChartDataSaver
    { 
        private IChartDataPathMaker _pathMaker;
          
        public DataSaver(IChartDataPathMaker chartDataPathMaker)
        {
            _pathMaker = chartDataPathMaker;
        }

        public void Save(IEnumerable<ChartData1> enumerable)
        {
            var lines = enumerable.SelectMany(aa => aa.Candles).Select(aa => aa.ToString());

            File.WriteAllLines("s.txt", lines.ToArray());
        }

        public void Save(string symbol, string source, string timeframe, IEnumerable<Candle> candles)
        {
            string filePath = _pathMaker.GetPath(symbol, source, timeframe);
            var newLines = candles.Select(aa => aa.ToString());

            if (!File.Exists(filePath))
            {
                File.WriteAllLines(filePath, newLines);
                return;
            }

            var previousLines = File.ReadAllLines(filePath);
            File.WriteAllLines(filePath, newLines.Concat(previousLines));
        }
    }
}