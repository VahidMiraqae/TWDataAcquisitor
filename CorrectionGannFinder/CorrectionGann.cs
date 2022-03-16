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
            var symbol = "BTCUSDT"; 

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

            var startPrice = 3156.52;
            var startDateTime = new DateTime(2017, 12, 17);

            var input = new SearchInput(60000, 500000, startDateTime, new DateTime(2021, 1, 1), new DateTime(2023, 12, 1), 300, 300);

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


            var ordered = matchList.OrderByDescending(item => item.Score).Select(item =>
            {
                var aa = ToDateTime(item.Fan.StartBar, startDateTime, timeframe).ToString("MMM dd, yyyy HH:mm");
                var p = item.Fan.TargetPrice;
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
