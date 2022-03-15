using System.Collections.Generic;
using System.Threading.Tasks;

namespace TWDataAcquisitor
{
    internal class TestSession : IRecordingSession
    {
        private int _counter;
        private ILatestChartDataReader dataExist;

        public TestSession(ILatestChartDataReader dataExist)
        {
            this.dataExist = dataExist;
        }

        public bool IsRunning { get; private set; } = true;

        public string Title => "the title";

        public string Symbol => throw new System.NotImplementedException();

        public string Source => throw new System.NotImplementedException();

        public string Timeframe => throw new System.NotImplementedException();

        public bool AnyData => throw new System.NotImplementedException();

        public async Task AcquireData()
        {
            await Task.Delay(1000);

            _counter++;

            if (_counter == 2)
            {
                IsRunning = false;
            }
        }

        public Task CorrectDateTimes()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Candle> GetCandles()
        {
            return null;
        }

        public Task Process()
        {
            throw new System.NotImplementedException();
        }
    }
}