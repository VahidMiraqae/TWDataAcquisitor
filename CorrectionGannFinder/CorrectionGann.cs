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
            var timeframe = Timeframes.Hour4;

            var filePath = _pathMaker.GetPath(symbol, source, timeframe);

            var candles = CandleReader.ReadCandles(filePath).ToArray();

            var startPrice = 2.0;
            var startDateTime = candles.First().Datetime;

            var matchList = new List<GannFanMatch>();

            var from = (int)((new DateTime(2022, 7, 1) - startDateTime).TotalMinutes / _timeFrame[timeframe]);
            var to = (int)((new DateTime(2023, 7, 1) - startDateTime).TotalMinutes / _timeFrame[timeframe]);

            for (int time = from; time <= to; time += 4)
            {
                for (double price = 60; price <= 300; price += 5)
                {

                    var fan = new GannFan(startPrice, time, price);

                    var fanMatch = GannFanMatcher.Match(fan, candles);

                    matchList.Add(fanMatch);
                }
            }

            var ordered = matchList.OrderByDescending(aa => aa.Score).ToList();


            foreach (var item in ordered)
            {
                var aa = item.Fan.Time - 3136;
                var p = item.Fan.Price;

            }
        }
    }
}
