using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TWCommon;
using TWDataAcquisitor;

internal class ChartData1
{
    public List<CandleWrapper> Candles { get; } = new List<CandleWrapper>();
    public string Timeframe { get; }
    public string Symbol { get; set; }
    public string Source { get; set; }
    public bool EndOfData { get; set; }
    private IOcrProcess _ocrProcessor;
    private string[] _lines;

    public ChartData1(string[] lines, IOcrProcess ocrProcessor)
    {
        _ocrProcessor = ocrProcessor;

        _lines = lines;
        Symbol = lines[0];
        Timeframe = lines[1];
        Source = lines[2];

    }

    public void AssignDateTimes()
    {
        var firstCandle = Candles.FirstOrDefault();
        var firstDateTimeStr = _ocrProcessor.Ocr(firstCandle.DateTimeImage).Result;

        DateTime? firstDateTime = ClassA.TextToDateTime(firstDateTimeStr, firstCandle.DateTimeImage);

        if (firstDateTime == null)
        {
            var datetime = GetCurrentCandleDateTime(Timeframe);
            firstCandle.Candle.Datetime = datetime.Subtract(Timeframes.Durations[Timeframe]);
        }
        else
        {
            firstCandle.Candle.Datetime = firstDateTime.Value;
        }

        for (int i = 1; i < Candles.Count; i++)
        {
            Candles[i].Candle.Datetime = Candles[i - 1].Candle.Datetime.Subtract(Timeframes.Durations[Timeframe]);
        }
    }

    private IEnumerable<string> GetDataLines(string[] lines)
    {
        int skips = lines[3].StartsWith("O") ? 3 : 4;
        
        if (lines.Skip(skips).Take(1).FirstOrDefault().Split('|')[0] == lines.Skip(skips + 1).Take(1).FirstOrDefault().Split('|')[0])
        {
            return lines.Skip(skips + 1);
        }
        else
        {
            return lines.Skip(skips);
        }


    }

    public async Task ParseCandles()
    {
        await Task.Run(() =>
        {
            foreach (var line in  GetDataLines(_lines))
            {
                if (line.StartsWith("On/a"))
                {
                    EndOfData = true;
                    break;
                }
                var split = line.Split('|');
                var candle = new Candle(split[0]);
                var candleW = new CandleWrapper(candle, split[1]);
                // System.IO.File.WriteAllBytes(split[0] + ".png", candleW.DateTimeImage);
                Candles.Add(candleW);
            }
        });
    }

    public static DateTime GetCurrentCandleDateTime(string timeframe)
    {
        var input = DateTime.UtcNow;
        var b = Timeframes.Durations[timeframe];

        var a = (int)((input.Hour * 60 + input.Minute) / b.TotalMinutes);
        return input.StartOfDay().Add(TimeSpan.FromSeconds(a * b.TotalSeconds));
    }


    
}
