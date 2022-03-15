using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWCommon
{
    public class CandleWrapper
    {
        public CandleWrapper(Candle candle, string v)
        {
            Candle = candle;
            DateTimeImage = Convert.FromBase64String(v);
        } 

        public Candle Candle { get; set; }
        public byte[] DateTimeImage { get; set; }
    }
}
