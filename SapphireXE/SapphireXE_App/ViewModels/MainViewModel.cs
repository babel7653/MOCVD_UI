using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using CsvHelper;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SapphireXE_App.Commands;
using SapphireXE_App.Commons;
using SapphireXE_App.Models;
using TwinCAT.Ads;

namespace SapphireXE_App.ViewModels
{
  public class MainViewModel : ViewModelBase
  {
    public bool btnPlotAnalog { get; set; } = true;
    public bool btnPlotTemperature { get; set; } = true;
    public bool btnPlotPressure { get; set; } = true;
    public bool btnPlotTotalFlow { get; set; } = true;
    TcAdsClient tcClient = new TcAdsClient();
    AdsStream dataStream = new AdsStream(1);
    AmsNetId amsNetId = new AmsNetId("10.10.10.10.1.1");
    int hVal = 0;
    Random rand = new Random();

    bool progressChart = false;


    private int _value;
    public int tcValueToRead
    {
      get { return _value; }
      set { SetProperty(ref _value, value); }
    }

    public List<RecipeRun> RunData { get; set; }
    public List<Event> Events { get; set; }
    List<LineSeries> Yata = new();

    private int timerCount = 0;
    private int eventCount = 0;
    private int MaxSecondsToLiveShow = 60;
    private DispatcherTimer? _timer;
    public BindableCollection<RecipeControlData> RecipeLiveData { get; set; }
    public BindableCollection<RecipeControlData> RecipeProgressData { get; set; }
    public BindableCollection<Event> EventData { get; set; } = new();
    public PlotModel RecipeLivePlot { get; set; }
    public PlotModel RecipeProgressPlot { get; set; }

    OxyPlotManager PlotManager1 = new();
    OxyPlotManager PlotManager2 = new();


    public ICommand StartAcquisitionCommand => new RelayCommand(StartAcquisition);
    public ICommand StopAcquisitionCommand => new RelayCommand(StopAcquisition);
    public ICommand BtnPlotAnalogCommand => new RelayCommand(PlotAnalog);
    public ICommand BtnPlotTemperatureCommand => new RelayCommand(PlotTemperature);
    public ICommand BtnPlotPressureCommand => new RelayCommand(PlotPressure);
    public ICommand BtnPlotTotalFlowCommand => new RelayCommand(PlotTotalFlow);

    /// <summary>
    /// chart display
    /// </summary>
    private void PlotAnalog() => btnPlotAnalog = !btnPlotAnalog;

    private void PlotTemperature() => btnPlotTemperature = !btnPlotTemperature;

    private void PlotPressure() => btnPlotPressure = !btnPlotPressure;

    private void PlotTotalFlow() => btnPlotTotalFlow = !btnPlotTotalFlow;


    public MainViewModel()
    {
      //ConnectPLC();

      LoadData();
      EventLoad();
      LivePlotSetting();
      ProgressPlotSetting();
      StartAcquisition();

      RecipeLiveData = new BindableCollection<RecipeControlData>();
      RecipeLiveData.CollectionChanged += RecipeLiveData_CollectionChanged;

      RecipeProgressData = new BindableCollection<RecipeControlData>();
      RecipeProgressData.CollectionChanged += RecipeProgressData_CollectionChanged;

    }

    private void ConnectPLC()
    {
      try
      {
        tcClient.Connect(amsNetId, 851);
        if (tcClient.IsConnected)
        {
          Console.WriteLine("Connected");
          hVal = tcClient.CreateVariableHandle("MAIN.test_out");
          tcValueToRead = (int)tcClient.ReadAny(hVal, typeof(int));
          Console.WriteLine($"{tcValueToRead}");
        }
        else
        {
          MessageBox.Show("Not Connected", "Connection State");
        }
      }
      catch (Exception err)
      {
        MessageBox.Show(err.Message, "PLC Connect");
      }
    }

    public void LivePlotSetting()
    {
      string[] lineTitle =
      {
        "Analog", "Temperature", "Pressure", "TotalFlow",
        "AnalogSet", "TemperatureSet", "PressureSet", "TotalFlowSet",
      };
      OxyColor[] lineColor =
      {
        OxyColors.Yellow, OxyColors.Red, OxyColors.Green, OxyColors.SkyBlue,
        OxyColors.DarkRed, OxyColors.DarkCyan, OxyColors.DarkBlue, OxyColors.DarkGreen
      };
      MarkerType[] lineMaker =
      {
        MarkerType.None, MarkerType.None, MarkerType.None, MarkerType.None,
        MarkerType.None, MarkerType.None, MarkerType.None, MarkerType.None,
      };

      PlotManager1.SetDateTiemAxisX("TimeStamp", "HH:mm:ss");
      PlotManager1.SetAxisY("Data Value");
      PlotManager1.AddSeries(8, lineTitle, lineColor, lineMaker);
      RecipeLivePlot = PlotManager1.PlotModel;
    }

    public void ProgressPlotSetting()
    {
      string[] lineTitle =
      {
        "Analog", "Temperature", "Pressure", "TotalFlow",
        "AnalogSet", "TemperatureSet", "PressureSet", "TotalFlowSet",
      };
      OxyColor[] lineColor =
      {
        OxyColors.Yellow, OxyColors.Red, OxyColors.Green, OxyColors.SkyBlue,
        OxyColors.DarkRed, OxyColors.DarkCyan, OxyColors.DarkBlue, OxyColors.DarkGreen
      };
      MarkerType[] lineMaker =
      {
        MarkerType.None, MarkerType.None, MarkerType.None, MarkerType.None,
        MarkerType.None, MarkerType.None, MarkerType.None, MarkerType.None,
      };

      PlotManager2.SetDateTiemAxisX("TimeStamp", "HH:mm:ss");
      PlotManager2.SetAxisY("Data Value");
      PlotManager2.AddSeries(8, lineTitle, lineColor, lineMaker);
      RecipeProgressPlot = PlotManager2.PlotModel;
    }

    private void RecipeLiveData_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems is null) return;

      var dateTimeAxis = RecipeLivePlot.Axes.OfType<DateTimeAxis>().First();
      var series1 = RecipeLivePlot.Series.OfType<LineSeries>().ElementAt(0);
      var series2 = RecipeLivePlot.Series.OfType<LineSeries>().ElementAt(1);
      var series3 = RecipeLivePlot.Series.OfType<LineSeries>().ElementAt(2);
      var series4 = RecipeLivePlot.Series.OfType<LineSeries>().ElementAt(3);

      if (!series1.Points.Any())
      {
        dateTimeAxis.Minimum = DateTimeAxis.ToDouble(DateTime.Now);
        dateTimeAxis.Maximum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(MaxSecondsToLiveShow));
      }

      foreach (var newItem in e.NewItems)
      {
        if (newItem is RecipeControlData liveData)
        {
          series1.Points.Add(new DataPoint(DateTimeAxis.ToDouble(liveData.TimeStamp), liveData.Data1));
          series2.Points.Add(new DataPoint(DateTimeAxis.ToDouble(liveData.TimeStamp), liveData.Data2));
          series3.Points.Add(new DataPoint(DateTimeAxis.ToDouble(liveData.TimeStamp), liveData.Data3));
          series4.Points.Add(new DataPoint(DateTimeAxis.ToDouble(liveData.TimeStamp), liveData.Data4));
        }
      }

      // if (series.Points.Last().X > dateTimeAxis.Maximum)
      if (DateTimeAxis.ToDateTime(series1.Points.Last().X) > DateTimeAxis.ToDateTime(dateTimeAxis.Maximum))
      {
        dateTimeAxis.Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(-1 * MaxSecondsToLiveShow));
        dateTimeAxis.Maximum = DateTimeAxis.ToDouble(DateTime.Now);
        dateTimeAxis.Reset();
      }
      series1.IsVisible = btnPlotAnalog;
      series2.IsVisible = btnPlotTemperature;
      series3.IsVisible = btnPlotPressure;
      series4.IsVisible = btnPlotTotalFlow;

      PlotManager1.PlotModel.InvalidatePlot(true);

      OnPropertyChanged(nameof(RecipeLivePlot));
    }

    private void RecipeProgressData_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems is null) return;

      var dateTimeAxis = RecipeProgressPlot.Axes.OfType<DateTimeAxis>().First();
      var series1 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(0);
      var series2 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(1);
      var series3 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(2);
      var series4 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(3);
      var series5 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(0);
      var series6 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(1);
      var series7 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(2);
      var series8 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(3);

      if (!progressChart)
      {
        List<List<double>> lineData = GetRecipeSetData();
        var totalTimeSpan = lineData[2].Count;

        dateTimeAxis.Minimum = DateTimeAxis.ToDouble(DateTime.Now);
        dateTimeAxis.Maximum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(totalTimeSpan));
        for (int i = 0; i < totalTimeSpan; i++)
        {
          series5.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(i)), lineData[0][i]));
          series6.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(i)), lineData[1][i]));
          series7.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(i)), lineData[2][i]));
          series8.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(i)), lineData[3][i]));
        }
        progressChart = true;
      }

      foreach (var newItem in e.NewItems)
      {
        if (newItem is RecipeControlData liveData)
        {
          series1.Points.Add(new DataPoint(DateTimeAxis.ToDouble(liveData.TimeStamp), liveData.Data1));
          series2.Points.Add(new DataPoint(DateTimeAxis.ToDouble(liveData.TimeStamp), liveData.Data2));
          series3.Points.Add(new DataPoint(DateTimeAxis.ToDouble(liveData.TimeStamp), liveData.Data3));
          series4.Points.Add(new DataPoint(DateTimeAxis.ToDouble(liveData.TimeStamp), liveData.Data4));
        }
      }

      series1.IsVisible = btnPlotAnalog;
      series2.IsVisible = btnPlotTemperature;
      series3.IsVisible = btnPlotPressure;
      series4.IsVisible = btnPlotTotalFlow;
      series5.IsVisible = btnPlotAnalog;
      series6.IsVisible = btnPlotTemperature;
      series7.IsVisible = btnPlotPressure;
      series8.IsVisible = btnPlotTotalFlow;

      PlotManager2.PlotModel.InvalidatePlot(true);

      OnPropertyChanged(nameof(RecipeProgressPlot));
    }

    public List<List<double>> GetRecipeSetData()
    {
      List<List<double>> YData = new()
      {
        new List<double>() { 0 },
        new List<double>() { 0 },
        new List<double>() { 0 },
        new List<double>() { 0 }
      };
      double[] y_init = { 0, 0, 0, 0 };
      double[] slope = { 0, 0, 0, 0 };
      List<double> y = new() { 0, 0, 0, 0 };
      DateTime now = DateTime.Now;

      //string Filepath = OpenCsv;
      string Filepath = "..\\..\\..\\data\\recipe\\_test_recipe progroress_data.csv";
      if (ValidateCsvFilePath(Filepath))
      {
        using var streamReader = new StreamReader(Filepath);
        using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
        csvReader.Read();
        while (csvReader.Read())
        {
          Recipe r = new()
          {
            rTime = csvReader.GetField<int>(2),
            hTime = csvReader.GetField<int>(3),
            rTemp = csvReader.GetField<int>(4),
            rPressure = csvReader.GetField<int>(5),
            sRotation = csvReader.GetField<int>(6),
            cTemp = csvReader.GetField<int>(7),
            Loop = csvReader.GetField<int>(8),
            Jump = csvReader.GetField<int>(9),
            M01 = csvReader.GetField<int>(10),
            M02 = csvReader.GetField<int>(11),
            M03 = csvReader.GetField<int>(12),
            M04 = csvReader.GetField<int>(13),
          };

          slope[0] = (r.rTemp - y_init[0]) / r.rTime;
          slope[1] = (r.rPressure - y_init[1]) / r.rTime;
          slope[2] = (r.sRotation - y_init[2]) / r.rTime;
          slope[3] = (r.cTemp - y_init[3]) / r.rTime;

          for (int i = 0; i < 4; i++)
          {
            for (int j = 0; j < r.rTime; j++)
            {
              y[i] = y_init[i] + slope[i];
              YData[i].Add(y[i]);

              y_init[i] = y[i];
            }
            for (int k = 0; k < r.hTime; k++)
            {
              YData[i].Add(y[i]);
            }
          }
        }
      }
      return YData;
    }

    public bool CanStartAcquisition => _timer?.IsEnabled != true;
    public void StartAcquisition()
    {

      if (_timer is null)
      {
        _timer = new DispatcherTimer
        {
          Interval = TimeSpan.FromMilliseconds(1000),
        };

        _timer.Tick += MockSensorRecievedData;
        _timer.Tick += RisingEventData;
      }

      _timer.Start();
      OnPropertyChanged(nameof(CanStartAcquisition));
      OnPropertyChanged(nameof(CanStopAcquisition));
    }

    private void MockSensorRecievedData(object? sender, EventArgs e)
    {
      try
      {
        hVal = tcClient.CreateVariableHandle("MAIN.test_out");
        tcValueToRead = (int)tcClient.ReadAny(hVal, typeof(int));
      }
      catch
      {
        tcValueToRead = default;
      }

      RecipeLiveData.Add(new()
      {
        TimeStamp = DateTime.Now,
        //Data1 = RunData[timerCount].PvE01,
        //Data2 = RunData[timerCount].PvE02,
        //Data3 = RunData[timerCount].SvE01,
        //Data4 = RunData[timerCount].SvE02,
        Data1 = tcValueToRead * (20 + rand.NextDouble()),
        Data2 = 10 * Math.Cos(tcValueToRead / 100) * tcValueToRead + 1500,
        Data3 = (300 - tcValueToRead) * (20 + rand.NextDouble()),
        Data4 = 10 * Math.Cos(tcValueToRead / 120) * tcValueToRead + 1800,

      });
      RecipeProgressData.Add(new()
      {
        TimeStamp = DateTime.Now,
        Data1 = tcValueToRead * (20 + rand.NextDouble()),
        Data2 = 10 * Math.Cos(tcValueToRead / 100) * tcValueToRead + 1500,
        Data3 = (300 - tcValueToRead) * (20 + rand.NextDouble()),
        Data4 = 10 * Math.Cos(tcValueToRead / 120) * tcValueToRead + 1800,
      });

      timerCount += 1;
    }


    public bool CanStopAcquisition => _timer?.IsEnabled == true;
    public void StopAcquisition()
    {
      _timer?.Stop();
      OnPropertyChanged(nameof(CanStartAcquisition));
      OnPropertyChanged(nameof(CanStopAcquisition));
    }

    public void LoadData()
    {
      RunData = new();
      //string Filepath = OpenCsv;
      string Filepath = "..\\..\\..\\data\\dataLog\\newformat_20240228_162236.csv";
      if (ValidateCsvFilePath(Filepath))
      {
        using var streamReader = new StreamReader(Filepath);
        using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
        csvReader.Read();
        csvReader.Read();

        while (csvReader.Read())
        {
          RecipeRun r = new()
          {
            DateTime = csvReader.GetField<DateTime>(0),
            PvE01 = csvReader.GetField<double>(1),
            PvE02 = csvReader.GetField<double>(2),
            PvE03 = csvReader.GetField<double>(3),
            PvE04 = csvReader.GetField<double>(4),
            PvE05 = csvReader.GetField<double>(5),
            PvE06 = csvReader.GetField<double>(6),
            PvE07 = csvReader.GetField<double>(7),
            PvIH = csvReader.GetField<double>(8),
            PvIH_CW = csvReader.GetField<double>(9),
            PvIH_KW = csvReader.GetField<double>(10),
            PvLH1 = csvReader.GetField<double>(11),
            PvLH2 = csvReader.GetField<double>(12),
            PvLH3 = csvReader.GetField<double>(13),
            PvLH4 = csvReader.GetField<double>(14),
            PvLH5 = csvReader.GetField<double>(15),
            PvLH6 = csvReader.GetField<double>(16),
            PvLH7 = csvReader.GetField<double>(17),
            PvLH8 = csvReader.GetField<double>(18),
            PvM01 = csvReader.GetField<double>(19),
            PvM02 = csvReader.GetField<double>(20),
            PvM03 = csvReader.GetField<double>(21),
            PvM04 = csvReader.GetField<double>(22),
            PvM05 = csvReader.GetField<double>(23),
            PvM06 = csvReader.GetField<double>(24),
            PvM07 = csvReader.GetField<double>(25),
            PvM08 = csvReader.GetField<double>(26),
            PvM09 = csvReader.GetField<double>(27),
            PvM10 = csvReader.GetField<double>(28),
            PvM11 = csvReader.GetField<double>(29),
            PvM12 = csvReader.GetField<double>(30),
            PvM13 = csvReader.GetField<double>(31),
            PvM14 = csvReader.GetField<double>(32),
            PvM15 = csvReader.GetField<double>(33),
            PvM16 = csvReader.GetField<double>(34),
            PvM17 = csvReader.GetField<double>(35),
            PvM18 = csvReader.GetField<double>(36),
            PvM19 = csvReader.GetField<double>(37),
            PvP01 = csvReader.GetField<double>(38),
            PvP02 = csvReader.GetField<double>(39),
            PvP03 = csvReader.GetField<double>(40),
            PvPOS = csvReader.GetField<double>(41),
            PvROT = csvReader.GetField<double>(42),
            PvS01 = csvReader.GetField<double>(43),
            PvS02 = csvReader.GetField<double>(44),
            PvS03 = csvReader.GetField<double>(45),
            PvS04 = csvReader.GetField<double>(46),
            PvS05 = csvReader.GetField<double>(47),
            PvSH_CW = csvReader.GetField<double>(48),
            PvTB1 = csvReader.GetField<double>(49),
            PvTB2 = csvReader.GetField<double>(50),
            PvTB3 = csvReader.GetField<double>(51),
            PvTB4 = csvReader.GetField<double>(52),
            PvTB5 = csvReader.GetField<double>(53),
            PvTB6 = csvReader.GetField<double>(54),
            SvE01 = csvReader.GetField<double>(55),
            SvE02 = csvReader.GetField<double>(56),
            SvE03 = csvReader.GetField<double>(57),
            SvE04 = csvReader.GetField<double>(58),
            SvE05 = csvReader.GetField<double>(59),
            SvE06 = csvReader.GetField<double>(60),
            SvE07 = csvReader.GetField<double>(61),
            SvIH = csvReader.GetField<double>(62),
            SvM01 = csvReader.GetField<double>(63),
            SvM02 = csvReader.GetField<double>(64),
            SvM03 = csvReader.GetField<double>(65),
            SvM04 = csvReader.GetField<double>(66),
            SvM05 = csvReader.GetField<double>(67),
            SvM06 = csvReader.GetField<double>(68),
            SvM07 = csvReader.GetField<double>(69),
            SvM08 = csvReader.GetField<double>(70),
            SvM09 = csvReader.GetField<double>(71),
            SvM10 = csvReader.GetField<double>(72),
            SvM11 = csvReader.GetField<double>(73),
            SvM12 = csvReader.GetField<double>(74),
            SvM13 = csvReader.GetField<double>(75),
            SvM14 = csvReader.GetField<double>(76),
            SvM15 = csvReader.GetField<double>(77),
            SvM16 = csvReader.GetField<double>(78),
            SvM17 = csvReader.GetField<double>(79),
            SvM18 = csvReader.GetField<double>(80),
            SvM19 = csvReader.GetField<double>(81),
            SvP01 = csvReader.GetField<double>(82),
            SvPOS = csvReader.GetField<double>(83),
            SvROT = csvReader.GetField<double>(84),
          };
          RunData.Add(r);
        }
      }
    }


    /// <summary>
    /// 경로를 검증한다.
    /// true : 옳바른 경로이다. false : 부적절한 경로이다.
    /// </summary>
    private bool ValidateCsvFilePath(string filePath)
    {
      if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Datagrid에 이벤트 데이터 전달
    /// </summary>
    private void EventLoad()
    {
      Events = new();
      // load csv data and test 
      string Filename = default;
      //csvFilename = OpenCsv;
      Filename = "..\\..\\..\\data\\eventLog\\_screen_datagrid_data.csv";

      using StreamReader streamReader = new StreamReader(Filename);
      using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
      csvReader.Read();
      csvReader.Read();
      while (csvReader.Read())
      {
        Event e = new()
        {
          State = csvReader.GetField<string>(0),
          Content = csvReader.GetField<string>(1),
          Date = csvReader.GetField<string>(2),
          Time = csvReader.GetField<string>(3),
          Checked = csvReader.GetField<string>(4),
        };
        Events.Add(e);
      }
    }

    private void RisingEventData(object? sender, EventArgs e)
    {
      Random random = new();
      int rand = random.Next(10);
      if (rand == 3 && eventCount < Events.Count - 3)
      {
        eventCount += 1;
        EventData.Add(new()
        {
          State = Events[eventCount].State,
          Content = Events[eventCount].Content,
          Date = Events[eventCount].Date,
          Time = Events[eventCount].Time,
          Checked = Events[eventCount].Checked,
        });
      }
    }




  }
}
