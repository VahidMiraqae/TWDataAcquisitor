using System;

public class Candle
{ 
    public DateTime Datetime { get; set; }
    public float Open { get; private set; }
    public float High { get; private set; }
    public float Low { get; private set; }
    public float Close { get; private set; }
    public float Volume { get; private set; }

    public Candle(string ohlcvol)
    {
        // NewMethod(ohlcvol);

        NewMethod1(ohlcvol);

    }

    public Candle(DateTime dateTime, float open, float high, float low, float close, float vol)
    {
        Datetime = dateTime;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = vol;
    }

    public override string ToString()
    {
        return $"{Datetime:MMM dd, yyyy HH:mm}\t{Open}\t{High}\t{Low}\t{Close}\t{Volume}";
    }

    private void NewMethod1(string ohlcvol)
    {
        var OInd = ohlcvol.IndexOf("O");
        var HInd = ohlcvol.IndexOf("H");
        var LInd = ohlcvol.IndexOf("L");
        var CInd = ohlcvol.IndexOf("C");
        var VolInd = ohlcvol.IndexOf("Vol");

        var tt = ohlcvol.Substring(OInd + 1, HInd - OInd - 1);
        Open = float.Parse(tt);

        High = float.Parse(ohlcvol.Substring(HInd + 1, LInd - HInd - 1));

        Low = float.Parse(ohlcvol.Substring(LInd + 1, CInd - LInd - 1));

        if (VolInd == -1)
        {
            Close = float.Parse(ohlcvol.Substring(CInd + 1));
        }
        else
        {
            Close = float.Parse(ohlcvol.Substring(CInd + 1, VolInd - CInd - 1));

            var rr = ohlcvol.Substring(VolInd + 3);

            var multiplier = 1;
            var toLow = rr.ToLower();
            var justNumbers = toLow;

            if (toLow.EndsWith("k"))
            {
                multiplier = 1000;
                justNumbers = justNumbers.Substring(0, justNumbers.Length - 1);
            }
            else if (toLow.EndsWith("m"))
            {
                multiplier = 1000000;
                justNumbers = justNumbers.Substring(0, justNumbers.Length - 1);
            }
            else if (toLow.EndsWith("b"))
            {
                multiplier = 1000000000;
                justNumbers = justNumbers.Substring(0, justNumbers.Length - 1);
            }

            Volume = multiplier * float.Parse(justNumbers);
        }
         
    }

    public static Candle Parse(string line)
    {
        var split = line.Split('\t');
        var dateTime = DateTime.Parse(split[0]);
        var open = float.Parse(split[1]);
        var high = float.Parse(split[2]);
        var low = float.Parse(split[3]);
        var close = float.Parse(split[4]);
        var vol = float.Parse(split[5]);
        var candle = new Candle(dateTime, open, high, low, close, vol);
        return candle;
    }

    private void NewMethod(string ohlcvol)
    {
        var split = ohlcvol.Split(' ');


        Open = float.Parse(split[0].Substring(1));

        High = float.Parse(split[1].Substring(1));

        Low = float.Parse(split[2].Substring(1));

        Close = float.Parse(split[3].Substring(1));

        if (split.Length == 5)
        {
            var multiplier = 1;
            var toLow = split[4].ToLower();
            var justNumbers = toLow.Substring(3);

            if (toLow.EndsWith("k"))
            {
                multiplier = 1000;
                justNumbers = justNumbers.Substring(0, justNumbers.Length - 1);
            }
            else if (toLow.EndsWith("m"))
            {
                multiplier = 1000000;
                justNumbers = justNumbers.Substring(0, justNumbers.Length - 1);
            }
            else if (toLow.EndsWith("b"))
            {
                multiplier = 1000000000;
                justNumbers = justNumbers.Substring(0, justNumbers.Length - 1);
            }

            Volume = multiplier * float.Parse(justNumbers);

        }
    }
}