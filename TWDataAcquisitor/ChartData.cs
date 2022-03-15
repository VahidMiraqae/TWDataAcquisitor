using IronOcr;
using System;
using System.Collections.Generic;
using System.Linq;
using TWCommon;

internal class ChartData
{
    public List<CandleWrapper> Candles { get; } = new List<CandleWrapper>();
    public string Timeframe { get; }
    public string Symbol { get; set; }
    public string Source { get; set; }
    public bool EndOfData { get; set; }
   


    public ChartData(string[] lines, DateTime beforeCurrentCandleDatatime)
    {

        if ((lines.Length - 4) % 10 == 0)
        {
            Symbol = lines[0];
            Timeframe = lines[1];
            Source = lines[2];

            if(lines.Skip(4).Take(1).FirstOrDefault() == lines.Skip(5).Take(1).FirstOrDefault())
            {
                ReadCandles(lines.Skip(5));
            }
            else
            {
                ReadCandles(lines.Skip(4));
            }


            var datetime = GetCurrentCandleDateTime(beforeCurrentCandleDatatime, Timeframe);


            SetDateTime(Candles.FirstOrDefault(), datetime.Subtract(pairs[Timeframe]));

            Candles.FirstOrDefault().Candle.Datetime = datetime.Subtract(pairs[Timeframe]);

            for (int i = 1; i < Candles.Count; i++)
            {
                SetDateTime(Candles[i], Candles[i - 1].Candle.Datetime.Subtract(pairs[Timeframe]));
            }


        }
    }

    public ChartData(string[] lines, IronTesseract ocr)
    {

        if ((lines.Length - 4) % 10 == 0)
        {
            Symbol = lines[0];
            Timeframe = lines[1];
            Source = lines[2];

            if (lines.Skip(4).Take(1).FirstOrDefault() == lines.Skip(5).Take(1).FirstOrDefault())
            {
                ReadCandles(lines.Skip(5));
            }
            else
            {
                ReadCandles(lines.Skip(4));
            }

            Candles.FirstOrDefault()
            var datetime = GetCurrentCandleDateTime(beforeCurrentCandleDatatime, Timeframe);


            SetDateTime(Candles.FirstOrDefault(), datetime.Subtract(pairs[Timeframe]));

            Candles.FirstOrDefault().Candle.Datetime = datetime.Subtract(pairs[Timeframe]);

            for (int i = 1; i < Candles.Count; i++)
            {
                SetDateTime(Candles[i], Candles[i - 1].Candle.Datetime.Subtract(pairs[Timeframe]));
            }


        }
    }

    private void SetDateTime(CandleWrapper candleWrapper, DateTime dateTime)
    {
        candleWrapper.Candle.Datetime = dateTime;
    }

    private void ReadCandles(IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            if (line.StartsWith("On/a"))
            {
                EndOfData = true;
                break;
            }
            var split = line.Split('|');
            var candle = new Candle(split[0]);
            var candleW = new CandleWrapper(candle, split[1]);
            Candles.Add(candleW);
        }
    }

    private static DateTime GetCurrentCandleDateTime(DateTime input, string timeframe)
    {
        var b = pairs[timeframe];

        var a = (int)((input.Hour * 60 + input.Minute) / b.TotalMinutes);
        return input.StartOfDay().Add(TimeSpan.FromSeconds(a * b.TotalSeconds));
    }


    private static Dictionary<string, TimeSpan> pairs = new Dictionary<string, TimeSpan>()
    {
        {"1D", TimeSpan.FromDays(1) },
        {"4h", TimeSpan.FromHours(4) },
        {"3h", TimeSpan.FromHours(3) },
        {"2h", TimeSpan.FromHours(2) },
        {"1h", TimeSpan.FromHours(1) },
        {"45", TimeSpan.FromMinutes(45) },
        {"30", TimeSpan.FromMinutes(30) },
        {"15", TimeSpan.FromMinutes(15) },
        {"5", TimeSpan.FromMinutes(5) },
    };


}