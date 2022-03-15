using System;
using System.Collections;
using System.Collections.Generic;
using TWCommon;

namespace CorrectionGannFinder
{
    internal class SearchInput
    { 

        public SearchInput(double fromPrice, double toPrice, DateTime startDateTime, DateTime fromDateTime, DateTime toDateTime, int priceCount = 100, int dateTimeCount = 100)
        {
            FromPrice = fromPrice;
            ToPrice = toPrice;
            FromDateTime = fromDateTime;  
            ToDateTime = toDateTime;
            PriceCount = priceCount;
            DateTimeCount = dateTimeCount;
            StartDateTime = startDateTime;
        }

        public double FromPrice { get; }
        public double ToPrice { get; }
        public DateTime FromDateTime { get; }
        public DateTime ToDateTime { get; }
        public int PriceCount { get; set; }
        public int DateTimeCount { get; set; }
        public DateTime StartDateTime { get; }

        public IEnumerable<int> GetBars(string timeFrame)
        {
            var offset = (int)((FromDateTime - StartDateTime).TotalMinutes / Timeframes.Durations[timeFrame].TotalMinutes);

            var count = (int)((ToDateTime - FromDateTime).TotalMinutes / Timeframes.Durations[timeFrame].TotalMinutes);
            var rem = count / DateTimeCount;
            var step = rem == 0 ? 1 : rem;

            var count1 = count >= DateTimeCount ? DateTimeCount : count;

            for (int i = 0; i < count1; i++)
            {
                yield return i * step + offset;
            }
        }

        public IEnumerable<double> GetPrices()
        {
            var steps = (ToPrice - FromPrice) / (PriceCount - 1);

            for (int i = 0; i < PriceCount; i++)
            {
                yield return i * steps + FromPrice;
            }
        }
    }
}