using System;
using System.Collections;
using System.Collections.Generic;

namespace CorrectionGannFinder
{
    internal class GannFan
    { 
        public GannFan(double startPrice, int time, double price1)
        {
            StartPrice = startPrice;
            Time = time;
            Price = price1;
        }

        private static double[] _multipliers = new double[]
        {
            1.0/8.0, 1.0/4.0, 1.0/3.0, 1.0/2.0,
            1.0/1.0,
            8.0/1.0, 4.0/1.0, 3.0/1.0, 2.0/1.0,
        };

        public double StartPrice { get; }
        public int Time { get; }
        public double Price { get; }

        public IEnumerable<(double a, double b)> GetFans()
        {
            var m = (Price - StartPrice) / -Time;

            for (int i = 0; i < _multipliers.Length; i++)
            {
                var a = _multipliers[i] * m;
                var b = StartPrice - a * Time;
                yield return (a, b);
            } 
        }

    }
}