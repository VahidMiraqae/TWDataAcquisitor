using System.Collections.Generic;
using System.Threading.Tasks;

namespace TWDataAcquisitor
{
    internal interface IRecordingSession
    {
        bool IsRunning { get; }
        string Title { get; }
        string Symbol { get; }
        string Source { get; }
        string Timeframe { get; }
        bool AnyData { get; }

        Task AcquireData();
        IEnumerable<Candle> GetCandles();
        Task CorrectDateTimes();
    }
}