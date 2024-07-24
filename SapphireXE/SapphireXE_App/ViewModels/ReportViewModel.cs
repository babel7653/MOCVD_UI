using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CsvHelper;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;
using SapphireXE_App.Views;
using SapphireXE_App.Commons;
using SapphireXE_App.Models;
using Caliburn.Micro;
using SapphireXE_App.Commands;
using OxyPlot.Axes;
using System.Linq;

namespace SapphireXE_App.ViewModels
{
  class ReportViewModel : ViewModelBase
  {
    public BindableCollection<RecipeControlData> RecipeCompareData { get; set; } = new();
    public BindableCollection<RecipeControlData> RecipeCompareData1 { get; set; } = new();
    public BindableCollection<RecipeControlData> RecipeCompareData2 { get; set; } = new();
    public PlotModel RecipeComparePlot { get; set; }
    public List<RecipeRun> RunData { get; set; } = new();
    public string logFilepath1 { get; set; }
    public string logFilepath2 { get; set; }

    public bool btnPlotData1 { get; set; } = true;
    public bool btnPlotData2 { get; set; } = true;
    public bool btnPlotData3 { get; set; } = true;
    public bool btnPlotData4 { get; set; } = true;
    public bool btnPlotDevice { get; set; } = true;

    public ICommand PlotRecipeFileOpenCommand => new RelayCommand(PlotRecipeFileOpen);
    public ICommand PlotRecipeFileRemoveCommand => new RelayCommand(PlotRecipeFileRemove);
    public ICommand PlotLogFileOpen1Command => new RelayCommand(PlotLogFileOpen1);
    public ICommand PlotLogFileOpen2Command => new RelayCommand(PlotLogFileOpen2);
    public ICommand PlotLogFileRemove1Command => new RelayCommand(PlotLogFileRemove1);
    public ICommand PlotLogFileRemove2Command => new RelayCommand(PlotLogFileRemove2);
    public ICommand btnPlotData1Command => new RelayCommand(BtnPlotData1);
    public ICommand btnPlotData2Command => new RelayCommand(BtnPlotData2);
    public ICommand btnPlotData3Command => new RelayCommand(BtnPlotData3);
    public ICommand btnPlotData4Command => new RelayCommand(BtnPlotData4);
    public ICommand btnPlotDeviceCommand => new RelayCommand(BtnPlotDevice);


    /// <summary>
    /// 임시 : Plot Device 버튼으로 datagrid에 표시되는 device 1, 2의 데이터 전환
    /// </summary>
    private void BtnPlotDevice()
    {
      btnPlotDevice = !btnPlotDevice;
      RecipeCompareData = btnPlotDevice ? RecipeCompareData1 : RecipeCompareData2;
      PlotData();
    }


    /// <summary>
    /// 생성자
    /// </summary>
    public ReportViewModel()
    {
    }


    private void BtnPlotData1()
    {
      if (RecipeCompareData1 == null) return;
      btnPlotData1 = !btnPlotData1;
      PlotData();
    }

    private void BtnPlotData2()
    {
      if (RecipeCompareData1 == null) return;
      btnPlotData2 = !btnPlotData2;
      PlotData();
    }

    private void BtnPlotData3()
    {
      if (RecipeCompareData2 == null) return;
      btnPlotData3 = !btnPlotData3;
      PlotData();
    }

    private void BtnPlotData4()
    {
      if (RecipeCompareData2 == null) return;
      btnPlotData4 = !btnPlotData4;
      PlotData();
    }

    public void InitializePlotModel()
    {
      RecipeComparePlot = new()
      {
        Title = "Demo Live Tracking",
      };

      RecipeComparePlot.Axes.Add(new DateTimeAxis
      {
        Title = "TimeStamp",
        Position = AxisPosition.Bottom,
        StringFormat = "HH:mm:ss",
        IntervalLength = 60,
        IsPanEnabled = true,
        IsZoomEnabled = true,
        IntervalType = DateTimeIntervalType.Seconds,
        MajorGridlineStyle = LineStyle.Solid,
        MinorGridlineStyle = LineStyle.Solid,
      });

      RecipeComparePlot.Axes.Add(new LinearAxis
      {
        Title = "Data Value",
        Position = AxisPosition.Left,
        IsPanEnabled = true,
        IsZoomEnabled = true,
      });

      RecipeComparePlot.Series.Add(new LineSeries()
      {
        MarkerType = MarkerType.Circle,
      });

      RecipeComparePlot.Series.Add(new LineSeries()
      {
        MarkerType = MarkerType.Cross,
      });

      RecipeComparePlot.Series.Add(new LineSeries()
      {
        MarkerType = MarkerType.Star,
      });

      RecipeComparePlot.Series.Add(new LineSeries()
      {
        MarkerType = MarkerType.Diamond,
      });
      RecipeComparePlot.Series.Add(new LineSeries()
      {
        MarkerType = MarkerType.Triangle,
      });

      RecipeComparePlot.Series.Add(new LineSeries()
      {
        MarkerType = MarkerType.Plus,
      });
      RecipeComparePlot.Series.Add(new LineSeries()
      {
        MarkerType = MarkerType.Square,
      });

      RecipeComparePlot.Series.Add(new LineSeries()
      {
        MarkerType = MarkerType.Star,
      });
    }

    private void PlotData()
    {
      CultureInfo culture = new CultureInfo("kr-KR");
      InitializePlotModel();
      var series1 = RecipeComparePlot.Series.OfType<LineSeries>().ElementAt(0);
      var series2 = RecipeComparePlot.Series.OfType<LineSeries>().ElementAt(1);
      var series3 = RecipeComparePlot.Series.OfType<LineSeries>().ElementAt(2);
      var series4 = RecipeComparePlot.Series.OfType<LineSeries>().ElementAt(3);
      var series5 = RecipeComparePlot.Series.OfType<LineSeries>().ElementAt(4);
      var series6 = RecipeComparePlot.Series.OfType<LineSeries>().ElementAt(5);
      var series7 = RecipeComparePlot.Series.OfType<LineSeries>().ElementAt(6);
      var series8 = RecipeComparePlot.Series.OfType<LineSeries>().ElementAt(7);

      //var dateTimeAxis = RecipeComparePlot.Axes.OfType<DateTimeAxis>().First();
      double sec = 0;
      foreach (var compareData in RecipeCompareData1)
      {
        DateTime t = new();
        series1.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.Data1));
        series2.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.Data2));
        series3.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.Data3));
        series4.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.Data4));
        sec += 1;
      }
      sec = 0;
      foreach (var compareData in RecipeCompareData2)
      {
        DateTime t = new();
        series5.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.Data5));
        series6.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.Data6));
        series7.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.Data7));
        series8.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.Data8));
        sec += 1;
      }

      series1.IsVisible = btnPlotData1;
      series2.IsVisible = btnPlotData2;
      series3.IsVisible = btnPlotData3;
      series4.IsVisible = btnPlotData4;
      series5.IsVisible = btnPlotData1;
      series6.IsVisible = btnPlotData2;
      series7.IsVisible = btnPlotData3;
      series8.IsVisible = btnPlotData4;
      RecipeComparePlot.InvalidatePlot(true);

      OnPropertyChanged(nameof(RecipeCompareData));
      OnPropertyChanged(nameof(RecipeCompareData1));
      OnPropertyChanged(nameof(RecipeCompareData2));
      OnPropertyChanged(nameof(RecipeComparePlot));

    }

    public BindableCollection<RecipeControlData> SelectData()
    {
      BindableCollection<RecipeControlData> bc = new();
      if (RunData.Count > 0)
      {
        foreach (RecipeRun r in RunData)
        {
          RecipeControlData s = new()
          {
            TimeStamp = r.DateTime,
            Data1 = r.PvE01,
            Data2 = r.PvE02,
            Data3 = r.PvE03,
            Data4 = r.PvE04,
            Data5 = r.SvE01,
            Data6 = r.SvE02,
            Data7 = r.SvE03,
            Data8 = r.SvE04,
          };
          bc.Add(s);
        }
      }
      return bc;
    }


    public BindableCollection<RecipeControlData> LoadData(string filename)
    {
      BindableCollection<RecipeControlData> bc = new();

      OpenFileDialog csv = new();
      if (ValidateFilePath(filename))
      {
        using var streamReader = new StreamReader(filename);
        using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
        csvReader.Read();

        while (csvReader.Read())
        {
          RecipeControlData r = new()
          {
            TimeStamp = csvReader.GetField<DateTime>(0),
            Data1 = csvReader.GetField<double>(1),
            Data2 = csvReader.GetField<double>(2),
            Data3 = csvReader.GetField<double>(3),
            Data4 = csvReader.GetField<double>(4),
            Data5 = csvReader.GetField<double>(55),
            Data6 = csvReader.GetField<double>(56),
            Data7 = csvReader.GetField<double>(57),
            Data8 = csvReader.GetField<double>(58),
          };
          bc.Add(r);
        }
      }
      return bc;
    }



    private void PlotRecipeFileOpen()
    {
      MessageBox.Show("PlotRecipeFileOpen");
    }

    private void PlotRecipeFileRemove()
    {
      MessageBox.Show("PlotRecipeFileRemove");
    }

    public void PlotLogFileOpen1()
    {
      PlotLogFileRemove1();  //2개 chart를 구현하면 지울 것

      logFilepath1 = OpenFile();
      OnPropertyChanged(nameof(logFilepath1));

      RecipeCompareData1 = LoadData(logFilepath1);
      PlotData();

    }

    public void PlotLogFileOpen2()
    {
      PlotLogFileRemove2();  //2개 chart를 구현하면 지울 것
      logFilepath2 = OpenFile();
      OnPropertyChanged(nameof(logFilepath2));

      RecipeCompareData2 = LoadData(logFilepath2);
      PlotData();
    }

    private void PlotLogFileRemove1()
    {
      logFilepath1 = "";
      RecipeCompareData1 = new();
      PlotData();

      OnPropertyChanged(nameof(logFilepath1));
    }

    private void PlotLogFileRemove2()
    {
      logFilepath2 = "";
      RecipeCompareData2 = new();
      PlotData();

      OnPropertyChanged(nameof(logFilepath2));
    }

    /// <summary>
    /// Open file
    /// </summary>
    /// <returns></returns>
    private static string OpenFile()
    {
      OpenFileDialog file = new();
      return file.ShowDialog() != true ? null : file.FileName;
    }

    /// <summary>
    /// 경로를 검증한다.
    /// true : 옳바른 경로이다.
    /// false : 부적절한 경로이다.
    /// </summary>
    private bool ValidateFilePath(string filePath)
    {
      if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return false;
      return true;
    }
  }
}
