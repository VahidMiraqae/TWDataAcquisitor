namespace TWCommon
{
    public interface IChartDataPathMaker
    {
        string ChartDataDir { get; }

        string GetPath(string symbol, string source, string timeframe);
    }
}