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
            var source = "BITSTAMP";
            var symbol = "BTCUSD"; 

            foreach (var timeframe in new[] {Timeframes.Hour1, Timeframes.Hour2, Timeframes.Hour4, Timeframes.Day1})
            {
                NewMethod(source, symbol, timeframe);
            }
        }

        private void NewMethod(string source, string symbol, string timeframe)
        {
            var candles = GetCandles(symbol, source, timeframe);

            if (!candles.Any())
            {
                return;
            }

            double startPrice = 152;
            var startDateTime = new DateTime(2015, 1, 14);

            var input = new SearchInput(1000, 30000, startDateTime, new DateTime(2017, 1, 1), new DateTime(2019, 1, 1), 100, 100);

            var neededCandles = candles.Where(aa => aa.Datetime >= startDateTime).ToArray();
            var gap = (int)((neededCandles[0].Datetime - startDateTime).TotalMinutes / Timeframes.Durations[timeframe].TotalMinutes);

            startPrice = Math.Log10(startPrice);

            var matchList = new List<GannFanMatch>();

            Parallel.ForEach(input.GetBars(timeframe), new ParallelOptions { MaxDegreeOfParallelism = 3 }, bar =>
            {
                Parallel.ForEach(input.GetPrices(), new ParallelOptions { MaxDegreeOfParallelism = 3 }, price =>
                {
                    price = Math.Log10(price);

                    var fans = new[] {
                        new GannFan(startPrice, bar, price, 0),
                        new GannFan(startPrice, 0, price, bar),
                        new GannFan(price, 0, startPrice, bar),
                        new GannFan(price, bar, startPrice, 0),
                    };

                    var fanMatch = GannFanMatcher.Match(fans, neededCandles, gap);

                    matchList.Add(fanMatch);

                });
            });


            var ordered = matchList.OrderByDescending(item => item.Score).Select(item =>
            {
                var aa = ToDateTime(item.Fans[0].StartBar, startDateTime, timeframe).ToString("MMM dd, yyyy HH:mm");
                var p = Math.Pow(10, item.Fans[0].TargetPrice);
                return $"{aa}\t{p}";
            }).ToList();


            var desk = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var path = Path.Combine(desk, $"matches_{timeframe}.txt");

            File.WriteAllLines(path, ordered);
        }

        private DateTime ToDateTime(int startBar, DateTime startDateTime, string timeframe)
        {
            return startDateTime.Add(TimeSpan.FromMinutes(Timeframes.Durations[timeframe].TotalMinutes * startBar));
        }

        private Candle[] GetCandles(string symbol, string source, string timeframe)
        { 
            var filePath = _pathMaker.GetPath(symbol, source, timeframe);

            if (!File.Exists(filePath))
            {
                return new Candle[0];
            }

            return CandleReader.ReadCandles(filePath).ToArray();
        }
    }
}
