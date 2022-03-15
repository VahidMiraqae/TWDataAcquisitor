using CorrectionGannFinder;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using TWCommon;

namespace TWDataAcquisitor
{
    internal class MainVm : BaseVm
    {
        private IChartDataPathMaker _chartDataPathMaker;
        private ILatestChartDataReader _latestDataReader;
        private IOcrProcess _ocrProcessor;
        private IChartDataSaver _dataSaver;
        private string _chartDataFilePath;
        private int _samplingDelay;
        private string _startBtnContent = START_BUTTON;
        private string _message = "Hi!";
        private bool _acquiring;
        private int _samplesPerRally;
        private CorrectionGann _correctionGannFinder;
        private const string START_BUTTON = "Start";

        public MainVm()
        {
            Initialize();
        }

        private void Initialize()
        {
            _chartDataPathMaker = new ChartDataPathMaker();
            _correctionGannFinder = new CorrectionGann(_chartDataPathMaker);
            _latestDataReader = new ChartDataReader(_chartDataPathMaker);
            //_ocrProcessor = new OcrClass();
            _ocrProcessor = new OcrClass1();
            _dataSaver = new DataSaver(_chartDataPathMaker);
            _chartDataFilePath = GetChartDataFilePath();
            _samplingDelay = GetSamplingDelay();
            _samplesPerRally = GetSamplesPerRally();
        }

        private int GetSamplesPerRally()
        {
            var valueStr = ConfigurationManager.AppSettings["SamplesPerRally"];
            if (string.IsNullOrWhiteSpace(valueStr))
            {
                return 30;
            }

            if (int.TryParse(valueStr, out int value))
            {
                return value;
            }

            return 30;

        }

        private int GetSamplingDelay()
        {
            var valueStr = ConfigurationManager.AppSettings["SmaplingDelayInMilliseconds"];
            if (string.IsNullOrWhiteSpace(valueStr))
            {
                return 100;
            }

            if (int.TryParse(valueStr, out int value))
            {
                return value;
            }

            return 100;
        }

        internal async Task StartCorrectionGannFinder()
        {
            await Task.Run(() => _correctionGannFinder.Start());
        }

        private string GetChartDataFilePath()
        {
            var path = ConfigurationManager.AppSettings["ChartDataFilePath"];
            if (string.IsNullOrWhiteSpace(path))
                path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "chart_data.txt");

            return path;
        }


        internal async Task OpenChartDataDir()
        {
            await Task.Run(() =>
            {
                Process.Start("explorer", $"\"{_chartDataPathMaker.ChartDataDir}\"");
            });
        }

        public string StartBtnContent { get => _startBtnContent; set { _startBtnContent = value; OnPropChanged(nameof(StartBtnContent)); } }
        public bool Acquiring { get => _acquiring; set { _acquiring = value; OnPropChanged(nameof(Acquiring)); } }
        public string Message { get => _message; set { _message = value; OnPropChanged(nameof(Message)); } }

        public bool Stopped { get; private set; }

        internal async Task Start()
        {
            Stopped = false;
            Acquiring = true;
            Message = "";
            IRecordingSession session = new ChartDataRecordingSession(_chartDataFilePath, _latestDataReader, _ocrProcessor);

            await CountDownToStart();

            if (Stopped)
            {
                DoStopped();
                return;
            }

            while (session.IsRunning)
            {
                if (Stopped)
                {
                    DoStopped();
                    return;
                }

                StartBtnContent = "Pressing Left ...";

                for (int i = 0; i < _samplesPerRally; i++)
                {
                    SendKeys.SendWait("{left}");
                    await Task.Delay(_samplingDelay);
                }

                if (Stopped)
                {
                    DoStopped();
                    return;
                }

                StartBtnContent = "Acquiring ...";

                await session.AcquireData();
            }

            StartBtnContent = "Processing ...";

            await session.CorrectDateTimes();

            if (!session.AnyData)
            {
                Message = $"No data acquired!";
                DoFinished();
                return;
            }

            if (Stopped)
            {
                DoStopped();
                return;
            }

            _dataSaver.Save(session.Symbol, session.Source, session.Timeframe, session.GetCandles());

            Message = $"{session.Title} Updated";

            DoFinished();
        }

        private void DoStopped()
        {
            Message = "Stopped!";
            DoFinished();
        }

        private void DoFinished()
        {
            Acquiring = false;
            StartBtnContent = START_BUTTON;
        }

        private async Task CountDownToStart()
        {
            for (int i = 5; i > 0; i--)
            {
                StartBtnContent = i.ToString();

                if (Stopped)
                    break;

                await Task.Delay(1000);
            }
        }

        internal async Task Stop()
        {
            Message = "Stopping ...";
            Stopped = true;
        }
    }
}