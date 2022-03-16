using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TWCommon
{
    public class ChartDataPathMaker : IChartDataPathMaker
    {
        private string _chartDataDir;

        public ChartDataPathMaker()
        {
            _chartDataDir = GetChartDataDir();
        }


        private string GetChartDataDir()
        {
            var dirName = "chart_data";
            var dir = ConfigurationManager.AppSettings["ChartDataDir"];
            var parentDir = dir;

            if (string.IsNullOrWhiteSpace(dir))
            {
                var assLocation = Assembly.GetExecutingAssembly().Location;
                parentDir = Path.GetDirectoryName(assLocation);
            }
            else if (dir.ToLower().Contains("desktop"))
            {
                parentDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            var ttt = Path.Combine(parentDir, dirName);
            Directory.CreateDirectory(ttt);
            return ttt;
        }

        public string ChartDataDir => _chartDataDir;

        public string GetPath(string symbol, string source, string timeframe)
        {
            var safeSource = RemoveBadChars(source);
            var sourceDir = Path.Combine(_chartDataDir, safeSource);
            Directory.CreateDirectory(sourceDir);

            var safeSymbol = RemoveBadChars(symbol);
            var symbolDir = Path.Combine(sourceDir, safeSymbol);
            Directory.CreateDirectory(symbolDir);

            return Path.Combine(symbolDir, timeframe + ".tsv");
        }

        private string RemoveBadChars(string source)
        {
            var inv1 = Path.GetInvalidPathChars();
            var inv2 = Path.GetInvalidFileNameChars();

            foreach (var item in inv1.Concat(inv2))
            {
                source = source.Replace(item, '_');
            }
            return source;
        }
    }
}
