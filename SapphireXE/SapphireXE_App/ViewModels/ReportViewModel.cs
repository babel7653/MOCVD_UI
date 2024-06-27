using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Win32;
using SapphireXE_App.Models;
using SapphireXE_App.Views;
using SkiaSharp;

namespace SapphireXE_App.ViewModels
{
  public class ReportViewModel : ViewModelBase
  {
    private static readonly List<double> _values1 = new();
    private static readonly List<double> _values2 = new();
    private static readonly List<double> _values3 = new();
    private static readonly List<double> _values4 = new();
    private static readonly List<double> _values5 = new();
    private static readonly List<double> _values6 = new();
    private static readonly List<double> _values7 = new();

    public Axis[] XAxes { get; set; }
    public object Sync { get; } = new object();
    List<RecipeRun> runData = new();


    public ISeries[] Series { get; set; } =
    {
      new LineSeries<double>
        {
          Values = (IEnumerable<double>)_values1,
          Name="PvE01",
          Stroke =  new SolidColorPaint(SKColors.Red){ StrokeThickness = 1 },
          Fill = null,
          GeometryFill = new SolidColorPaint(SKColors.Red),
          GeometryStroke = new SolidColorPaint(SKColors.Red),
          GeometrySize = 5,
          LineSmoothness= 3,
          ScalesYAt = 0 // it will be scaled at the Axis[0] instance 

        },
        new LineSeries<double>
        {
          Values = (IEnumerable<double>)_values2,
          Name="PvE02",
          Stroke =  new SolidColorPaint(SKColors.Magenta){ StrokeThickness = 1 },
          Fill = null,
          GeometryFill = new SolidColorPaint(SKColors.Magenta),
          GeometryStroke = new SolidColorPaint(SKColors.Magenta),
          GeometrySize = 5,
          LineSmoothness= 3,
          ScalesYAt = 0 // it will be scaled at the Axis[0] instance 

        },
        new LineSeries<double>
        {
          Values = (IEnumerable<double>)_values3,
          Name="PvE03",
          Stroke =  new SolidColorPaint(SKColors.Yellow){ StrokeThickness = 1 },
          Fill = null,
          GeometryFill = new SolidColorPaint(SKColors.Yellow),
          GeometryStroke = new SolidColorPaint(SKColors.Yellow),
          GeometrySize = 5,
          LineSmoothness= 3,
          ScalesYAt = 0 // it will be scaled at the Axis[0] instance 

        },
        new LineSeries<double>
        {
          Values = (IEnumerable<double>)_values4,
          Name="PvE04",
          Stroke =  new SolidColorPaint(SKColors.Green){ StrokeThickness = 1 },
          Fill = null,
          GeometryFill = new SolidColorPaint(SKColors.Green),
          GeometryStroke = new SolidColorPaint(SKColors.Green),
          GeometrySize = 5,
          LineSmoothness= 3,
          ScalesYAt = 0 // it will be scaled at the Axis[0] instance 

        },
        new LineSeries<double>
        {
          Values = (IEnumerable<double>)_values5,
          Name="PvE05",
          Stroke =  new SolidColorPaint(SKColors.Blue){ StrokeThickness = 1 },
          Fill = null,
          GeometryFill = new SolidColorPaint(SKColors.Blue),
          GeometryStroke = new SolidColorPaint(SKColors.Blue),
          GeometrySize = 5,
          LineSmoothness= 3,
          ScalesYAt = 0 // it will be scaled at the Axis[0] instance 

        },
        new LineSeries<double>
        {
          Values = (IEnumerable<double>)_values6,
          Name="PvE06",
          Stroke =  new SolidColorPaint(SKColors.Navy){ StrokeThickness = 1 },
          Fill = null,
          GeometryFill = new SolidColorPaint(SKColors.Navy),
          GeometryStroke = new SolidColorPaint(SKColors.Navy),
          GeometrySize = 5,
          LineSmoothness= 3,
          ScalesYAt = 0 // it will be scaled at the Axis[0] instance 

        },
        new LineSeries<double>
        {
          Values = (IEnumerable<double>)_values7,
          Name="PvE07",
          Stroke =  new SolidColorPaint(SKColors.Purple){ StrokeThickness = 1 },
          Fill = null,
          GeometryFill = new SolidColorPaint(SKColors.Purple),
          GeometryStroke = new SolidColorPaint(SKColors.Purple),
          GeometrySize = 5,
          LineSmoothness= 3,
          ScalesYAt = 0 // it will be scaled at the Axis[0] instance 

        }

    };

    public ICartesianAxis[] YAxes { get; set; } =
    {
        new Axis // the "unit" series will be scaled on this axis
        {
            Name = "pv_E values",
            NameTextSize = 14,
            NamePaint = new SolidColorPaint(SKColors.Red),
            NamePadding = new LiveChartsCore.Drawing.Padding(0, 20),
            Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
            TextSize = 12,
            LabelsPaint = new SolidColorPaint(SKColors.Red),
            TicksPaint = new SolidColorPaint(SKColors.Red),
            SubticksPaint = new SolidColorPaint(SKColors.Red),
            DrawTicksPath = true
        },
        //new Axis // the "hundreds" series will be scaled on this axis
        //{
        //    Name = "2nd Scale",
        //    NameTextSize = 14,
        //    NamePaint = new SolidColorPaint(SKColors.Red),
        //    NamePadding = new LiveChartsCore.Drawing.Padding(0, 20),
        //    Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
        //    TextSize = 12,
        //    LabelsPaint = new SolidColorPaint(SKColors.Red),
        //    TicksPaint = new SolidColorPaint(SKColors.Red),
        //    SubticksPaint = new SolidColorPaint(SKColors.Red),
        //    DrawTicksPath = true,
        //    ShowSeparatorLines = false,
        //    Position = LiveChartsCore.Measure.AxisPosition.End
        //},
        //new Axis // the "thousands" series will be scaled on this axis
        //{
        //    Name = "PvE03",
        //    NameTextSize = 14,
        //    NamePaint = new SolidColorPaint(SKColors.Red),
        //    NamePadding = new LiveChartsCore.Drawing.Padding(0, 20),
        //    Padding =  new LiveChartsCore.Drawing.Padding(0, 0, 20, 0),
        //    TextSize = 12,
        //    LabelsPaint = new SolidColorPaint(SKColors.Red),
        //    TicksPaint = new SolidColorPaint(SKColors.Red),
        //    SubticksPaint = new SolidColorPaint(SKColors.Red),
        //    DrawTicksPath = true,
        //    ShowSeparatorLines = false,
        //    Position = LiveChartsCore.Measure.AxisPosition.End
        //},

    };

    public SolidColorPaint LegendTextPaint { get; set; } =
        new SolidColorPaint
        {
          Color = new SKColor(50, 50, 50),
          SKTypeface = SKTypeface.FromFamilyName("Courier New")
        };

    public SolidColorPaint LedgendBackgroundPaint { get; set; } =
        new SolidColorPaint(new SKColor(240, 240, 240));

    public static List<double> Values1 => _values1;

    public static List<double> Values2 => _values2;

    public static List<double> Values3 => _values3;

    public static List<double> Values4 => _values4;

    public static List<double> Values5 => _values5;

    public static List<double> Values6 => _values6;

    public static List<double> Values7 => _values7;

    public void CsvData()
    {
      string csvFilename = default;

      OpenFileDialog csv = new();
      if (csv.ShowDialog() == true)
      {
        csvFilename = csv.FileName;
      }
      using var streamReader = new StreamReader(csvFilename);
      using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
      csvReader.Read();
      csvReader.Read();
      while (csvReader.Read())
      {

        RecipeRun r = new()
        {
          Step = csvReader.GetField<string>(0),
          PvE01 = csvReader.GetField<double>(1),
          PvE02 = csvReader.GetField<double>(2),
          PvE03 = csvReader.GetField<double>(3),
          PvE04 = csvReader.GetField<double>(4),
          PvE05 = csvReader.GetField<double>(5),
          PvE06 = csvReader.GetField<double>(6),
          PvE07 = csvReader.GetField<double>(7),
          DateTime = csvReader.GetField<string>(85)
        };
        runData.Add(r);
      }

      ViewChart();
    }

    private void ViewChart()
    {
      // Initialize chart data
      Values1.Clear();
      Values2.Clear();
      Values3.Clear();
      Values4.Clear();
      Values5.Clear();
      Values6.Clear();
      Values7.Clear();

      foreach (RecipeRun r in runData)
      {
        Values1.Add(r.PvE01);
        Values2.Add(r.PvE02);
        Values3.Add(r.PvE03);
        Values4.Add(r.PvE04);
        Values5.Add(r.PvE05);
        Values6.Add(r.PvE06);
        Values7.Add(r.PvE07);
      }
    }

  }

}
