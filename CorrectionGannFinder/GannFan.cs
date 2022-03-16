using System;
using System.Collections;
using System.Collections.Generic;

namespace CorrectionGannFinder
{
    internal class GannFan
    { 
        public GannFan(double startPrice, int startBar, double targetPrice, int targetBar)
        {
            StartPrice = startPrice;
            StartBar = startBar;
            TargetPrice = targetPrice;
            TargetBar = targetBar;
        }

        private static double[] _multipliers = new double[]
        {
            1.0/8.0, 1.0/5.0, 1.0/4.0, 1.0/3.0, 1.0/2.0,
            1.0/1.0,
            8.0/1.0, 5.0/1.0, 4.0/1.0, 3.0/1.0, 2.0/1.0,
        };

        public double StartPrice { get; }
        public int StartBar { get; }
        public double TargetPrice { get; }
        public int TargetBar { get; }

        public IEnumerable<(double a, double b)> GetFans() // y = ax + b
        {
            var m = (TargetPrice - StartPrice) / (TargetBar - StartBar);

            for (int i = 0; i < _multipliers.Length; i++)
            {
                var a = _multipliers[i] * m;
                var b = StartPrice - a * StartBar;
                yield return (a, b);
            } 
        }

    }
}