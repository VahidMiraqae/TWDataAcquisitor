using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWCommon;

namespace CorrectionGannFinder
{
    public class CorrectionGann
    {
        private IChartDataPathMaker _pathMaker;

        public CorrectionGann(IChartDataPathMaker pathMaker)
        {
            _pathMaker = pathMaker;
        }

        public void Start()
        {
            var source = "BINANCE";
            var symbol = "DOTUSDT";
            var timeframe = Timeframes.Hour2;

            var candles = GetCandles(symbol, source, timeframe);

            var startPrice = 2.0;
            var startDateTime = new DateTime(2020, 8, 19);

            var input = new SearchInput(60, 300, startDateTime, new DateTime(2022, 7, 1), new DateTime(2023, 1, 1),50,50);

            var neededCandles = candles.Where(aa => aa.Datetime >= startDateTime).ToArray();
            var gap = (int)((neededCandles[0].Datetime - startDateTime).TotalMinutes / Timeframes.Durations[timeframe].TotalMinutes);


            var matchList = new List<GannFanMatch>();

            Parallel.ForEach(input.GetBars(timeframe), new ParallelOptions { MaxDegreeOfParallelism = 3 }, bar =>
            {
                Parallel.ForEach(input.GetPrices(), new ParallelOptions { MaxDegreeOfParallelism = 3 }, price =>
                {

                    var fan = new GannFan(startPrice, bar, price);

                    var fanMatch = GannFanMatcher.Match(fan, neededCandles, gap);

                    matchList.Add(fanMatch);

                });
            });
             

            var ordered = matchList.OrderByDescending(aa => aa.Score).ToList();


            foreach (var item in ordered)
            {
                var aa = ToDateTime(item.Fan.StartBar, startDateTime, timeframe).ToString("MMM dd, yyyy HH:mm");
                var p = item.Fan.TargetPrice;

            }
        }

        private DateTime ToDateTime(int startBar, DateTime startDateTime, string timeframe)
        {
            return startDateTime.Add(TimeSpan.FromMinutes(Timeframes.Durations[timeframe].TotalMinutes * startBar));
        }

        private Candle[] GetCandles(string symbol, string source, string timeframe)
        { 
            var filePath = _pathMaker.GetPath(symbol, source, timeframe);

            return CandleReader.ReadCandles(filePath).ToArray();
        }
    }
}
