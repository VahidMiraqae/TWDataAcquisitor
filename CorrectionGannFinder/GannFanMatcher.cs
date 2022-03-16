using System;
using System.Linq;

namespace CorrectionGannFinder
{
    internal class GannFanMatcher
    {
        internal static GannFanMatch Match(GannFan fan, Candle[] candles, int gap)
        {
            var match = new GannFanMatch(fan);
            foreach ((double a, double b) in fan.GetFans())
            {
                int count = 0;

                for (int i = 0; i < candles.Length; i++)
                {
                    var dis = GetDistance(candles[i], gap + i, a, b); 
                    if (dis < 0.03)
                    {
                        count++;
                    }
                }

                match.Score += count;
            }
            return match;
        }

        private static double GetDistance(Candle candle, int i, double a, double b)
        {
            var openDis = Math.Abs(candle.Open - a * i - b) / Math.Sqrt(a * a + 1);
            var lowDis = Math.Abs(candle.Low - a * i - b) / Math.Sqrt(a * a + 1);
            var highDis = Math.Abs(candle.High - a * i - b) / Math.Sqrt(a * a + 1);
            var closeDis = Math.Abs(candle.Close - a * i - b) / Math.Sqrt(a * a + 1);
            return (new[] { openDis, lowDis, highDis, closeDis }).Min();
        }
    }
}