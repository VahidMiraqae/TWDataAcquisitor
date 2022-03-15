using System.Collections.Generic;

namespace TWDataAcquisitor
{
    internal interface IChartDataSaver
    {
        void Save(string symbol, string source, string timeframe, IEnumerable<Candle> candles);
    }
}