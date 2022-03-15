using System;

public interface ILatestChartDataReader
{
    Candle GetLatestData(string source, string symbol, string timeframe);
}