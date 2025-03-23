using OxyPlot.Series;
using OxyPlot;
using System.Drawing;

namespace SapphireXR_App.Models
{
    static class LogReportSeries
    {
        public enum LogNumber { One, Two };
        //public static Series GenerateSeries(string legendKey, string deviceName, LogNumber logNumber)
        //{
        //    Series series = new LineSeries()
        //    {
        //        Title = (logNumber == LogNumber.One ? "Log 1 " : "Log 2 ") + " " + name,
        //        Color = logNumber == LogNumber.One ? color.Item1 : color.Item2,
        //        MarkerStroke = OxyColors.Black,
        //        StrokeThickness = 1,
        //        MarkerType = MarkerType.None,
        //        MarkerSize = 2,
        //        LegendKey = legendKey
        //    };

         
        //    return series;
        //}
      

        private static OxyColor GenerateColor()
        {
            int index = Random.Shared.Next(AllKnownColors.Length);
            Color randomColor = Color.FromKnownColor(AllKnownColors[index]) ;

            return OxyColor.FromRgb(randomColor.R, randomColor.G, randomColor.B);
        }

        private static readonly KnownColor[] AllKnownColors = Enum.GetValues<KnownColor>();

        public static readonly Dictionary<string, (OxyColor, OxyColor)>  LogSeriesColor = new Dictionary<string, (OxyColor, OxyColor)> {
            { "PV_M01",( GenerateColor(),  GenerateColor()) }, { "SV_M01",( GenerateColor(),  GenerateColor()) },  { "PV_M02",( GenerateColor(),  GenerateColor()) }, { "SV_M02",( GenerateColor(),  GenerateColor()) }, 
            { "PV_M03",( GenerateColor(),  GenerateColor()) },  { "SV_M03",( GenerateColor(),  GenerateColor()) }, { "PV_M04",( GenerateColor(),  GenerateColor()) }, { "SV_M04",( GenerateColor(),  GenerateColor()) }, 
            { "PV_M05",( GenerateColor(),  GenerateColor()) }, { "SV_M05",( GenerateColor(),  GenerateColor()) }, { "PV_M06",( GenerateColor(),  GenerateColor()) }, { "SV_M06",( GenerateColor(),  GenerateColor()) }, 
            { "PV_M07",( GenerateColor(),  GenerateColor()) }, { "SV_M07",( GenerateColor(),  GenerateColor()) }, { "PV_M08",( GenerateColor(),  GenerateColor()) }, { "SV_M08",( GenerateColor(),  GenerateColor()) }, 
            { "PV_M09",( GenerateColor(),  GenerateColor()) },  { "SV_M09",( GenerateColor(),  GenerateColor()) }, { "PV_M10",( GenerateColor(),  GenerateColor()) }, { "SV_M10",( GenerateColor(),  GenerateColor()) },
            { "PV_M11",( GenerateColor(),  GenerateColor()) }, { "SV_M11",( GenerateColor(),  GenerateColor()) }, { "PV_M12",( GenerateColor(),  GenerateColor()) }, { "SV_M12",( GenerateColor(),  GenerateColor()) }, 
            { "PV_M13",( GenerateColor(),  GenerateColor()) }, { "SV_M13",( GenerateColor(),  GenerateColor()) }, { "PV_M14",( GenerateColor(),  GenerateColor()) }, { "SV_M14",( GenerateColor(),  GenerateColor()) }, 
            { "PV_M15",( GenerateColor(),  GenerateColor()) }, { "SV_M15",( GenerateColor(),  GenerateColor()) }, { "PV_M16",( GenerateColor(),  GenerateColor()) }, { "SV_M16",( GenerateColor(),  GenerateColor()) },
            { "PV_M17",( GenerateColor(),  GenerateColor()) }, { "SV_M17",( GenerateColor(),  GenerateColor()) }, { "PV_M18",( GenerateColor(),  GenerateColor()) }, { "SV_M18",( GenerateColor(),  GenerateColor()) },
            { "PV_M19",( GenerateColor(),  GenerateColor()) }, { "SV_M19",( GenerateColor(),  GenerateColor()) }, 
            { "PV_E01",( GenerateColor(),  GenerateColor()) }, { "SV_E01",( GenerateColor(),  GenerateColor()) }, { "PV_E02",( GenerateColor(),  GenerateColor()) }, { "SV_E02",( GenerateColor(),  GenerateColor()) },
            { "PV_E03",( GenerateColor(),  GenerateColor()) }, { "SV_E03",( GenerateColor(),  GenerateColor()) }, { "PV_E04",( GenerateColor(),  GenerateColor()) }, { "SV_E04",( GenerateColor(),  GenerateColor()) }, 
            { "PV_E05",( GenerateColor(),  GenerateColor()) }, { "SV_E05",( GenerateColor(),  GenerateColor()) }, { "PV_E06",( GenerateColor(),  GenerateColor()) }, { "SV_E06",( GenerateColor(),  GenerateColor()) },
            { "PV_E07",( GenerateColor(),  GenerateColor()) }, { "SV_E07",( GenerateColor(),  GenerateColor()) }, 
            { "PV_TEMP",( GenerateColor(),  GenerateColor()) }, { "SV_TEMP",( GenerateColor(),  GenerateColor()) }, { "PV_PRES",( GenerateColor(),  GenerateColor()) }, { "SV_PRES",( GenerateColor(),  GenerateColor()) }, 
            { "PV_ROT",( GenerateColor(),  GenerateColor()) }, { "SV_ROT",( GenerateColor(),  GenerateColor()) },  { "PV_IHT_KW",( GenerateColor(),  GenerateColor()) }, { "PV_SH_CW",( GenerateColor(),  GenerateColor()) }, { "PV_IHT_CW",( GenerateColor(),  GenerateColor()) }
        };
    }
}
