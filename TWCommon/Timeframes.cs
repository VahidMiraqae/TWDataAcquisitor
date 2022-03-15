using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWCommon
{
    public static class Timeframes
    {
        public static string Day1 { get; } = "1D";
        public static string Hour4 { get; } = "4h";
        public static string Hour3 { get; } = "3h";
        public static string Hour2 { get; } = "2h";
        public static string Hour1 { get; } = "1h";
        public static string Minute45 { get; } = "45";
        public static string Minute30 { get; } = "30";
        public static string Minute15 { get; } = "15";
        public static string Minute5 { get; } = "5";


        public static Dictionary<string, TimeSpan> Durations { get; } = new Dictionary<string, TimeSpan>()
        {
            {Day1, TimeSpan.FromDays(1) },
            {Hour4, TimeSpan.FromHours(4) },
            {Hour3, TimeSpan.FromHours(3) },
            {Hour2, TimeSpan.FromHours(2) },
            {Hour1, TimeSpan.FromHours(1) },
            {Minute45, TimeSpan.FromMinutes(45) },
            {Minute30, TimeSpan.FromMinutes(30) },
            {Minute15, TimeSpan.FromMinutes(15) },
            {Minute5, TimeSpan.FromMinutes(5) },
        };

    }
}
