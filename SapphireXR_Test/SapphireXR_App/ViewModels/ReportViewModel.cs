using Caliburn.Micro;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using Microsoft.Win32;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using SapphireXR_App.Models;
using SapphireXR_App.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Reflection.Metadata;
using System.Windows.Controls;


namespace SapphireXR_App.ViewModels
{
  public class ReportViewModel : ViewModelBase
  {
    private RelayCommand<object>? selectedItemsLeftPlotTagCommand;
    private RelayCommand<object>? selectedItemsRightPlotTagCommand;
    private RelayCommand<string>? cbCheckedPlotTagCommand;
    private RelayCommand<string>? cbUncheckedPlotTagCommand;
    private RelayCommand? btnMoveToLeftPlotTagCommand;
    private RelayCommand? btnMoveToRightPlotTagCommand;
    private RelayCommand? btnMoveToLeftPlotTagAllCommand;
    private RelayCommand? btnMoveToRightPlotTagAllCommand;

    private List<RecipeRun> RunData { get; set; } = [];

    private List<string> LeftTags { get; set; } = [];
    private List<string> RightTags { get; set; } = [];
    private List<string> _totalPlotTagList { get; set; } = [
      "PvE01",
      "PvE02",
      "PvE03",
      "PvE04",
      "PvE05",
      "PvE06",
      "PvE07",
      "PvIH",
      "PvIH_CW",
      "PvIH_KW",
      "PvLH1",
      "PvLH2",
      "PvLH3",
      "PvLH4",
      "PvLH5",
      "PvLH6",
      "PvLH7",
      "PvLH8",
      "PvM01",
      "PvM02",
      "PvM03",
      "PvM04",
      "PvM05",
      "PvM06",
      "PvM07",
      "PvM08",
      "PvM09",
      "PvM10",
      "PvM11",
      "PvM12",
      "PvM13",
      "PvM14",
      "PvM15",
      "PvM16",
      "PvM17",
      "PvM18",
      "PvM19",
      "PvP01",
      "PvP02",
      "PvP03",
      "PvPOS",
      "PvROT",
      "PvS01",
      "PvS02",
      "PvS03",
      "PvS04",
      "PvS05",
      "PvSH_CW",
      "PvTB1",
      "PvTB2",
      "PvTB3",
      "PvTB4",
      "PvTB5",
      "PvTB6",
      "SvE01",
      "SvE02",
      "SvE03",
      "SvE04",
      "SvE05",
      "SvE06",
      "SvE07",
      "SvIH",
      "SvM01",
      "SvM02",
      "SvM03",
      "SvM04",
      "SvM05",
      "SvM06",
      "SvM07",
      "SvM08",
      "SvM09",
      "SvM10",
      "SvM11",
      "SvM12",
      "SvM13",
      "SvM14",
      "SvM15",
      "SvM16",
      "SvM17",
      "SvM18",
      "SvM19",
      "SvP01",
      "SvPOS",
      "SvROT"];

    public string? logFilepath1 { get; set; }
    public string? logFilepath2 { get; set; }

    public ObservableCollection<string> LeftVisiablePlotTag { get; set; } = [];
    public ObservableCollection<string> RightVisiablePlotTag { get; set; } = [];
    public List<string> LeftSelectedTags { get; set; } = [];
    public List<string> RightSelectedTags { get; set; } = [];
    public bool IsCheckedPlotTagPv { get; set; } = true;
    public bool IsCheckedPlotTagSv { get; set; } = true;
    public bool IsCheckedPlotTagEtc { get; set; } = true;
    public bool btnPlotData1 { get; set; } = true;
    public bool btnPlotData2 { get; set; } = true;
    public bool btnPlotData3 { get; set; } = true;
    public bool btnPlotData4 { get; set; } = true;
    public bool btnPlotDevice { get; set; } = true;

    public ObservableCollection<RecipeRun> ReportCompareData { get; set; } = [];
    public ObservableCollection<RecipeRun> ReportCompareData1 { get; set; } = [];
    public ObservableCollection<RecipeRun> ReportCompareData2 { get; set; } = [];
    public PlotModel? ReportComparePlot { get; set; }

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

    public ICommand SelectedItemsLeftPlotTagCommand => selectedItemsLeftPlotTagCommand ??= new RelayCommand<object>(PerformSelectedItemsLeftPlotTag);
    public ICommand SelectedItemsRightPlotTagCommand => selectedItemsRightPlotTagCommand ??= new RelayCommand<object>(PerformSelectedItemsRightPlotTag);
    public ICommand CbCheckedPlotTagCommand => cbCheckedPlotTagCommand ??= new RelayCommand<string>(PerformCbCheckedPlotTag);
    public ICommand CbUncheckedPlotTagCommand => cbUncheckedPlotTagCommand??= new RelayCommand<string>(PerformCbUncheckedPlotTag);
    public ICommand BtnMoveToLeftPlotTagCommand => btnMoveToLeftPlotTagCommand ??= new RelayCommand(PerformBtnMoveToLeftPlotTag);
    public ICommand BtnMoveToRightPlotTagCommand => btnMoveToRightPlotTagCommand ??= new RelayCommand(PerformBtnMoveToRightPlotTag);
    public ICommand BtnMoveToLeftPlotTagAllCommand => btnMoveToLeftPlotTagAllCommand ??= new RelayCommand(PerformBtnMoveToLeftPlotTagAll);
    public ICommand BtnMoveToRightPlotTagAllCommand => btnMoveToRightPlotTagAllCommand ??= new RelayCommand(PerformBtnMoveToRightPlotTagAll);



    public ReportViewModel()
    {
      foreach (string item in _totalPlotTagList)
      {
        LeftVisiablePlotTag.Add(item);
        LeftTags.Add(item);
      }
    }


    private void PerformCbCheckedPlotTag(string content)
    {
      foreach (var tag in LeftTags)
      {
        if (tag.Substring(0, 2) == content && !LeftVisiablePlotTag.Contains(tag))
        {
          LeftVisiablePlotTag.Add(tag);
        }
      }

      foreach (var tag in RightTags)
      {
        if (tag.Substring(0, 2) == content && !RightVisiablePlotTag.Contains(tag))
        {
          RightVisiablePlotTag.Add(tag);
        }
      }

      OnPropertyChanged(nameof(LeftVisiablePlotTag));
      OnPropertyChanged(nameof(RightVisiablePlotTag));

    }

    private void PerformCbUncheckedPlotTag(string content)
    {
      foreach (var tag in LeftTags)
      {
        if (tag.Substring(0, 2) == content && LeftVisiablePlotTag.Contains(tag))
        {
          LeftVisiablePlotTag.Remove(tag);
        }
      }
      foreach (var tag in RightTags)
      {
        if (tag.Substring(0, 2) == content && RightVisiablePlotTag.Contains(tag))
        {
          RightVisiablePlotTag.Remove(tag);
        }
      }
      OnPropertyChanged(nameof(LeftVisiablePlotTag));
      OnPropertyChanged(nameof(RightVisiablePlotTag));

    }

    private void PerformSelectedItemsLeftPlotTag(object arg)
    {
      var eventArg = arg as SelectionChangedEventArgs;
      if (eventArg is null) { return; }
      if (eventArg.AddedItems.Count > 0)
      {
        var tag = eventArg.AddedItems[0] as string;
        if (!LeftSelectedTags.Contains(tag))
        {
          LeftSelectedTags.Add(tag);
        }
      }
      if (eventArg.RemovedItems.Count > 0)
      {
        var tag = eventArg.RemovedItems[0] as string;
        if (LeftSelectedTags.Contains(tag))
        {
          LeftSelectedTags.Remove(tag);
        }
      }
    }

    private void PerformSelectedItemsRightPlotTag(object arg)
    {
      var eventArg = arg as SelectionChangedEventArgs;
      if (eventArg is null) { return; }
      if (eventArg.AddedItems.Count > 0)
      {
        var tag = eventArg.AddedItems[0] as string;
        if (!RightSelectedTags.Contains(tag))
        {
          RightSelectedTags.Add(tag);
        }
      }
      if (eventArg.RemovedItems.Count > 0)
      {
        var tag = eventArg.RemovedItems[0] as string;
        if (!RightSelectedTags.Contains(tag))
        {
          RightSelectedTags.Remove(tag);
        }
      }
    }


    private void PerformBtnMoveToRightPlotTag()
    {
      if (LeftSelectedTags.Count == 0) return;

      List<string> tags = [];
      foreach (var tag in LeftSelectedTags) { tags.Add(tag); }

      foreach (var tag in tags)
      {
        LeftVisiablePlotTag.Remove(tag);
        RightVisiablePlotTag.Add(tag);
      }

      LeftTags.Clear();
      foreach (var tag in LeftVisiablePlotTag) { LeftTags.Add(tag); }
      RightTags.Clear();
      foreach (var tag in RightVisiablePlotTag) { RightTags.Add(tag); }

      OnPropertyChanged(nameof(LeftVisiablePlotTag));
      OnPropertyChanged(nameof(RightVisiablePlotTag));

      LeftSelectedTags.Clear();
    }


    private void PerformBtnMoveToLeftPlotTag()
    {
      if (RightSelectedTags.Count == 0) return;

      List<string> tags = [];
      foreach (var tag in RightSelectedTags) { tags.Add(tag); }

      foreach (var tag in tags)
      {
        LeftVisiablePlotTag.Add(tag);
        RightVisiablePlotTag.Remove(tag);
      }

      LeftTags.Clear();
      foreach (var tag in LeftVisiablePlotTag) { LeftTags.Add(tag); }
      RightTags.Clear();
      foreach (var tag in RightVisiablePlotTag) { RightTags.Add(tag); }

      OnPropertyChanged(nameof(LeftVisiablePlotTag));
      OnPropertyChanged(nameof(RightVisiablePlotTag));

      RightSelectedTags.Clear();

    }

    private void PerformBtnMoveToRightPlotTagAll()
    {
      LeftVisiablePlotTag.Clear();
      RightVisiablePlotTag.Clear();
      LeftTags.Clear();
      RightTags.Clear();
      foreach (var tag in _totalPlotTagList)
      {
        RightVisiablePlotTag.Add(tag);
        RightTags.Add(tag);
      }
      IsCheckedPlotTagPv = true;
      IsCheckedPlotTagSv = true;
      IsCheckedPlotTagEtc = true;
      OnPropertyChanged(nameof(IsCheckedPlotTagPv));
      OnPropertyChanged(nameof(IsCheckedPlotTagSv));
      OnPropertyChanged(nameof(IsCheckedPlotTagEtc));
      OnPropertyChanged(nameof(LeftVisiablePlotTag));
      OnPropertyChanged(nameof(RightVisiablePlotTag));
    }

    private void PerformBtnMoveToLeftPlotTagAll()
    {
      LeftVisiablePlotTag.Clear();
      RightVisiablePlotTag.Clear();
      LeftTags.Clear();
      RightTags.Clear();
      foreach (var tag in _totalPlotTagList)
      {
        LeftVisiablePlotTag.Add(tag);
        LeftTags.Add(tag);
      }
      IsCheckedPlotTagPv = true;
      IsCheckedPlotTagSv = true;
      IsCheckedPlotTagEtc = true;
      OnPropertyChanged(nameof(IsCheckedPlotTagPv));
      OnPropertyChanged(nameof(IsCheckedPlotTagSv));
      OnPropertyChanged(nameof(IsCheckedPlotTagEtc));
      OnPropertyChanged(nameof(LeftVisiablePlotTag));
      OnPropertyChanged(nameof(RightVisiablePlotTag));

    }


    /// <summary>
    /// 임시 : Plot Device 버튼으로 datagrid에 표시되는 device 1, 2의 데이터 전환
    /// </summary>
    private void BtnPlotDevice()
    {
      btnPlotDevice = !btnPlotDevice;
      ReportCompareData = btnPlotDevice ? ReportCompareData1 : ReportCompareData2;
      PlotData(RightVisiablePlotTag.Count);

    }


    /// <summary>
    /// 생성자
    /// </summary>


    private void BtnPlotData1()
    {
      if (ReportCompareData1 == null) return;
      btnPlotData1 = !btnPlotData1;
      PlotData(RightVisiablePlotTag.Count);
    }

    private void BtnPlotData2()
    {
      if (ReportCompareData1 == null) return;
      btnPlotData2 = !btnPlotData2;
      PlotData(RightVisiablePlotTag.Count);
    }

    private void BtnPlotData3()
    {
      if (ReportCompareData2 == null) return;
      btnPlotData3 = !btnPlotData3;
      PlotData(RightVisiablePlotTag.Count);
    }

    private void BtnPlotData4()
    {
      if (ReportCompareData2 == null) return;
      btnPlotData4 = !btnPlotData4;
      PlotData(RightVisiablePlotTag.Count);
    }

    public void InitializePlotModel(int numLines)
    {
      ReportComparePlot = new();
      ReportComparePlot.Axes.Add(new DateTimeAxis
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

      ReportComparePlot.Axes.Add(new LinearAxis
      {
        Title = "Data Value",
        Position = AxisPosition.Left,
        IsPanEnabled = true,
        IsZoomEnabled = true,
      });

      List<string> lineTitle = [];
      for (int i = 0; i < numLines; i++)
      {
        lineTitle.Add(i.ToString());
      }
      for (int i = 0; i < numLines; i++)
      {
        ReportComparePlot.Series.Add(new LineSeries()
        {
          Title = lineTitle[i],
          StrokeThickness = 1,
          MarkerType = MarkerType.None,
          MarkerSize = 2,
        });
      }
    }

    private void PlotData(int numLines)
    {
      InitializePlotModel(84);
      var series1 = ReportComparePlot.Series.OfType<LineSeries>().ElementAt(0);
      var series2 = ReportComparePlot.Series.OfType<LineSeries>().ElementAt(1);
      var series3 = ReportComparePlot.Series.OfType<LineSeries>().ElementAt(2);
      var series4 = ReportComparePlot.Series.OfType<LineSeries>().ElementAt(3);
      var series5 = ReportComparePlot.Series.OfType<LineSeries>().ElementAt(4);
      var series6 = ReportComparePlot.Series.OfType<LineSeries>().ElementAt(5);
      var series7 = ReportComparePlot.Series.OfType<LineSeries>().ElementAt(6);
      var series8 = ReportComparePlot.Series.OfType<LineSeries>().ElementAt(7);

      //var dateTimeAxis = ReportComparePlot.Axes.OfType<DateTimeAxis>().First();
      double sec = 0;
      foreach (var compareData in ReportCompareData1)
      {
        DateTime t = new();
        series1.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.PvE01));
        series2.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.PvE02));
        series3.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.PvE03));
        series4.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.PvE04));
        sec += 1;
      }
      sec = 0;
      foreach (var compareData in ReportCompareData2)
      {
        DateTime t = new();
        series5.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.SvE01));
        series6.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.SvE02));
        series7.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.SvE03));
        series8.Points.Add(new DataPoint(DateTimeAxis.ToDouble(t.AddSeconds(sec)), compareData.SvE04));
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
      ReportComparePlot.InvalidatePlot(true);

      OnPropertyChanged(nameof(ReportComparePlot));
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
      PlotLogFileRemove1();  // 이전 데이터 삭제

      string initDir = "D:\\sysnex\\mocvd\\MocvdNow\\SapphireXE\\data\\datalog\\";
      logFilepath1 = OpenFile(initDir);
      OnPropertyChanged(nameof(logFilepath1));
      ReportCompareData1 = LoadData(logFilepath1);
      //PlotData(RightVisiablePlotTag.Count);
      PlotData(8);
    }

    public void PlotLogFileOpen2()
    {
      PlotLogFileRemove2();  // 이전 데이터 삭제

      string initDir = "D:\\sysnex\\mocvd\\MocvdNow\\SapphireXE\\data\\datalog\\";
      logFilepath2 = OpenFile(initDir);
      OnPropertyChanged(nameof(logFilepath2));
      ReportCompareData2 = LoadData(logFilepath2);
      PlotData(RightVisiablePlotTag.Count);
    }

    private void PlotLogFileRemove1()
    {
      logFilepath1 = "";
      ReportCompareData1 = [];
      PlotData(RightVisiablePlotTag.Count);

      OnPropertyChanged(nameof(logFilepath1));
    }

    private void PlotLogFileRemove2()
    {
      logFilepath2 = "";
      ReportCompareData2 = [];
      
      PlotData(RightVisiablePlotTag.Count);

      OnPropertyChanged(nameof(logFilepath2));
    }

    public ObservableCollection<RecipeRun> LoadData(string filename)
    {
      ObservableCollection<RecipeRun> oc = [];
      var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
      {
        Delimiter = ",",
        HasHeaderRecord = true
      };

      using (StreamReader streamReader = new(filename))
      {
        using var csvReader = new CsvReader(streamReader, config);
        RunData = csvReader.GetRecords<RecipeRun>().ToList();
        int Count = RunData.Count;
      }
      foreach (var r in RunData)
      {
        oc.Add(r);
      }
      return oc;
    }


    /// <summary>
    /// Open file
    /// </summary>
    /// <returns></returns>
    private static string OpenFile(string initDir)
    {
      OpenFileDialog file = new()
      {
        InitialDirectory = initDir
      };
      return file.ShowDialog() != true ? null : file.FileName;
    }

  }
}
