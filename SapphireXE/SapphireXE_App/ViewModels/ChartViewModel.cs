using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Win32;
using SapphireXE_App.Models;
using SkiaSharp;

namespace SapphireXE_App.ViewModels
{
  public class ChartViewModel : ViewModelBase
  {
    public ObservableCollection<ISeries> Series { get; set; }
    public Axis[] XAxes { get; set; }
    public object Sync { get; } = new object();
    public bool IsReading { get; set; } = true;

    private DataTable _dataTable = new();
    public DataTable DataTable { get; set; }
    List<RecipeRun> runData = new();

    private readonly Random _random = new Random();
    private readonly List<DateTimePoint> _values1 = new List<DateTimePoint>();
    private readonly List<DateTimePoint> _values2 = new List<DateTimePoint>();
    private readonly List<DateTimePoint> _values3 = new List<DateTimePoint>();
    private readonly List<DateTimePoint> _values4 = new List<DateTimePoint>();
    private readonly List<DateTimePoint> _values5 = new List<DateTimePoint>();
    private readonly List<DateTimePoint> _values6 = new List<DateTimePoint>();
    private readonly List<DateTimePoint> _values7 = new List<DateTimePoint>();
    private readonly DateTimeAxis _customAxis;

    public List<RecipeRun> Data { get; set; }

    private string OpenCsv
    {
      get
      {
        Data = null;
        string csvFilename = default;

        OpenFileDialog csv = new();
        if (csv.ShowDialog() == true)
        {
          csvFilename = csv.FileName;
        }
        return csvFilename;
      }
    }



    public ChartViewModel()
    {
      Series = new ObservableCollection<ISeries>
      {
        new LineSeries<DateTimePoint>
        {
          Values = _values1,
          Stroke =  new SolidColorPaint(SKColors.Red){ StrokeThickness = 1 },
          Fill = null,
          GeometryFill = new SolidColorPaint(SKColors.Red),
          GeometryStroke = new SolidColorPaint(SKColors.Blue),
          GeometrySize = 5,
          LineSmoothness= 3,
        },
        new LineSeries<DateTimePoint>
        {
          Values = _values2,
          Stroke =  new SolidColorPaint(SKColors.Magenta){ StrokeThickness = 1 },
          Fill = null,
          GeometryFill = new SolidColorPaint(SKColors.Blue),
          GeometryStroke = new SolidColorPaint(SKColors.Blue),
          GeometrySize = 5,
          LineSmoothness= 3,
        },
        new LineSeries<DateTimePoint>
        {
          Values = _values3,
          Stroke =  new SolidColorPaint(SKColors.Yellow){ StrokeThickness = 1 },
          Fill = null,
          GeometryFill = new SolidColorPaint(SKColors.Green),
          GeometryStroke = new SolidColorPaint(SKColors.Blue),
          GeometrySize = 5,
          LineSmoothness= 3,
        },
        new LineSeries<DateTimePoint>
        {
          Values = _values4,
          Stroke =  new SolidColorPaint(SKColors.Green){ StrokeThickness = 1 },
          Fill = null,
          GeometryFill = new SolidColorPaint(SKColors.Red),
          GeometryStroke = new SolidColorPaint(SKColors.Blue),
          GeometrySize = 5,
          LineSmoothness= 3,
        },
        new LineSeries<DateTimePoint>
        {
          Values = _values5,
          Stroke =  new SolidColorPaint(SKColors.Blue){ StrokeThickness = 1 },
          Fill = null,
          GeometryFill = new SolidColorPaint(SKColors.Blue),
          GeometryStroke = new SolidColorPaint(SKColors.Blue),
          GeometrySize = 5,
          LineSmoothness= 3,
        },
        new LineSeries<DateTimePoint>
        {
          Values = _values6,
          Stroke =  new SolidColorPaint(SKColors.Navy){ StrokeThickness = 1 },
          Fill = null,
          GeometryFill = new SolidColorPaint(SKColors.Green),
          GeometryStroke = new SolidColorPaint(SKColors.Blue),
          GeometrySize = 5,
          LineSmoothness= 3,
        },
        new LineSeries<DateTimePoint>
        {
          Values = _values7,
          Stroke =  new SolidColorPaint(SKColors.Purple){ StrokeThickness = 1 },
          Fill = null,
          GeometryFill = new SolidColorPaint(SKColors.Green),
          GeometryStroke = new SolidColorPaint(SKColors.Blue),
          GeometrySize = 5,
          LineSmoothness= 3,

        }
      };


      float spanTime = 10.0f;

      _customAxis = new DateTimeAxis(TimeSpan.FromSeconds(1), Formatter)
      {
        CustomSeparators = GetSeparators(spanTime),
        AnimationsSpeed = TimeSpan.FromMilliseconds(0),
        SeparatorsPaint = new SolidColorPaint(SKColors.DarkRed.WithAlpha(120)),
        Padding = new LiveChartsCore.Drawing.Padding(-5, 0, -5, 10),

      };

      XAxes = new Axis[] { _customAxis };

      CsvDataLoad();

      _ = ViewTable();
      _ = ViewChart();

    }


    private void CsvDataLoad()
    {
      // load csv data and test 
      string csvFilename = default;

      csvFilename = OpenCsv;

      //var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
      //{
      //	Comment = '#',
      //	AllowComments = true,
      //	Delimiter = ";",
      //};

      using var streamReader = new StreamReader(csvFilename);
      using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
      csvReader.Read();
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
    }

    private async Task ViewTable()
    {
      // DataTable Columns setting
      _dataTable.Columns.Add("Step", typeof(string));
      _dataTable.Columns.Add("PvE01", typeof(string));
      _dataTable.Columns.Add("PvE02", typeof(string));
      _dataTable.Columns.Add("PvE03", typeof(string));
      _dataTable.Columns.Add("PvE04", typeof(string));
      _dataTable.Columns.Add("PvE05", typeof(string));
      _dataTable.Columns.Add("PvE06", typeof(string));
      _dataTable.Columns.Add("PvE07", typeof(string));
      _dataTable.Columns.Add("DateTime", typeof(string));

      foreach (RecipeRun r in runData)
      {
        await Task.Delay(1000);

        _ = _dataTable.Rows.Add(new string[] { $"{r.Step}", $"{r.PvE01}", $"{r.PvE02}", $"{r.PvE03}",
            $"{r.PvE04}", $"{r.PvE05}", $"{r.PvE06}", $"{r.PvE07}", $"{r.DateTime}"});

        DataTable = _dataTable;
        OnPropertyChanged(nameof(DataTable));
      }
    }


    private async Task ViewChart()
    {
      foreach (RecipeRun r in runData)
      {
        float spanTime = 20.0f;
        int delay = 1000;

        await Task.Delay(delay);

        _values1.Add(new DateTimePoint(DateTime.Now, r.PvE01));
        if (_values1.Count > spanTime * 1000 / delay) _values1.RemoveAt(0);
        _values2.Add(new DateTimePoint(DateTime.Now, r.PvE02));
        if (_values2.Count > spanTime * 1000 / delay) _values2.RemoveAt(0);
        _values3.Add(new DateTimePoint(DateTime.Now, r.PvE03));
        if (_values3.Count > spanTime * 1000 / delay) _values3.RemoveAt(0);
        _values4.Add(new DateTimePoint(DateTime.Now, r.PvE04));
        if (_values4.Count > spanTime * 1000 / delay) _values4.RemoveAt(0);
        _values5.Add(new DateTimePoint(DateTime.Now, r.PvE05));
        if (_values5.Count > spanTime * 1000 / delay) _values5.RemoveAt(0);
        _values6.Add(new DateTimePoint(DateTime.Now, r.PvE06));
        if (_values6.Count > spanTime * 1000 / delay) _values6.RemoveAt(0);
        _values7.Add(new DateTimePoint(DateTime.Now, r.PvE07));
        if (_values7.Count > spanTime * 1000 / delay) _values7.RemoveAt(0);

        _customAxis.CustomSeparators = GetSeparators(spanTime);
      }
    }


    private double[] GetSeparators(float t)
    {
      var now = DateTime.Now;

      return new double[]
      {
        now.AddSeconds(-t).Ticks,
        now.AddSeconds(-t/2).Ticks,
        now.Ticks
      };
    }

    private static string Formatter(DateTime date)
    {
      var secsAgo = (DateTime.Now - date).TotalSeconds;

      return secsAgo < 1
        ? "now"
        : $"{secsAgo:N0}s ago";
    }


    /// <summary>
    /// 경로를 검증한다.
    /// true : 옳바른 경로이다.
    /// false : 부적절한 경로이다.
    /// </summary>
    //private bool ValidateCsvFilePath(string filePath)
    //{
    //	if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
    //	{
    //		return false;
    //	}
    //	return true;
    //}
  }
}
