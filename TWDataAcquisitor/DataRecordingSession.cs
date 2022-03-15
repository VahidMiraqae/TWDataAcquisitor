using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using TWDataAcquisitor;

internal class ChartDataRecordingSession : IRecordingSession
{
    private IOcrProcess _ocrProcessor;
    private ILatestChartDataReader _lastestDataReader;

    private string _dataFilePath;
    public List<ChartData1> ChartDatas { get; set; } = new List<ChartData1>();
    public string Title
    {
        get
        {
            var first = ChartDatas.FirstOrDefault();
            return $"{first?.Symbol} - {first?.Source} - {first?.Timeframe}";
        }
    }


    private Candle _latestCandle;
    private bool _latestIsSet;

    public bool IsRunning { get; set; } = true;

    public string Symbol => ChartDatas.FirstOrDefault()?.Symbol;

    public string Source => ChartDatas.FirstOrDefault()?.Source;

    public string Timeframe => ChartDatas.FirstOrDefault()?.Timeframe;

    public bool AnyData => GetCandles().Any();

    public ChartDataRecordingSession(string chartDataFilePath, ILatestChartDataReader assesser, IOcrProcess ocrProcessor)
    {
        _ocrProcessor = ocrProcessor;
        _lastestDataReader = assesser;
        _dataFilePath = chartDataFilePath;
        if (File.Exists(_dataFilePath))
        {
            File.Delete(_dataFilePath);
        }
    }


    public async Task AcquireData()
    {
        for (int i = 0; i < 20; i++)
        {
            if (!File.Exists(_dataFilePath))
            {
                await Task.Delay(200);
            }
            else
            {
                break;
            }
        }

        if (!File.Exists(_dataFilePath))
        {
            IsRunning = false;
            return;
        }

        string[] lines = null;

        await Task.Run(() =>
        {
            lines = File.ReadAllLines(_dataFilePath);
        });

        var chartData = new ChartData1(lines, _ocrProcessor);

        await chartData.ParseCandles();

        chartData.AssignDateTimes();

        ChartDatas.Add(chartData);

        Task.Run(() =>
        {
            Thread.Sleep(500);
            SafeDelete(_dataFilePath);
        });

        if (!_latestIsSet)
        {
            _latestCandle = _lastestDataReader.GetLatestData(chartData.Source, chartData.Symbol, chartData.Timeframe);
            _latestIsSet = true;
        }

        var lastDateTime = chartData.Candles.Last().Candle.Datetime;

        if (_latestCandle != null && lastDateTime <= _latestCandle.Datetime)
        {
            foreach (var item in chartData.Candles.Where(aa => aa.Candle.Datetime <= _latestCandle.Datetime).ToList())
            {
                chartData.Candles.Remove(item);
            }
            IsRunning = false;
            return;
        }

        IsRunning = !chartData.EndOfData;
    }

    private void SafeDelete(string dataFilePath)
    {
        if (File.Exists(dataFilePath))
        {
            File.Delete(dataFilePath);
        }
    }

    public IEnumerable<Candle> GetCandles()
    {  
        return ChartDatas.SelectMany(aa => aa.Candles.Select(bb => bb.Candle));
    }

    public async Task CorrectDateTimes()
    {
        foreach (var chartData in ChartDatas)
        {
            var lastCandle = chartData.Candles.LastOrDefault();

            if (lastCandle == null)
            {
                continue;
            }
            var lastDatetimeStr = await _ocrProcessor.Ocr(lastCandle.DateTimeImage);

            DateTime? lastDatetime = ClassA.TextToDateTime(lastDatetimeStr, lastCandle.DateTimeImage);

            if (lastDatetime.Value == lastCandle.Candle.Datetime)
            {
                continue;
            }

            foreach (var candle in chartData.Candles)
            {
                var ttStr = await _ocrProcessor.Ocr(candle.DateTimeImage);

                DateTime? tt = ClassA.TextToDateTime(ttStr, candle.DateTimeImage);

                candle.Candle.Datetime = tt.Value;
            }

        }
    }



}
