using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using CsvHelper.Configuration;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SapphireXR_App.Bases;
using SapphireXR_App.Models;


namespace SapphireXR_App.ViewModels
{
  //Bottom Dashboard ViewModel
  public class BottomViewModel : ViewModelBase
  {
    public static DispatcherTimer? _timer;
    public static int timerCount = 0;
    public static int eventCount = 0;

    public static bool tcPlcState { get; set; }
    public static byte tcDeviceArray { get; set; }
    public static double tcTempCV { get; set; }
    public static double tcPressCV { get; set; }
    public static float tcRotateSV { get; set; }

    public static bool btnPlotAnalog { get; set; } = true;
    public static bool btnPlotTemperature { get; set; } = true;
    public static bool btnPlotPressure { get; set; } = true;
    public static bool btnPlotTotalFlow { get; set; } = true;
    public static string btnRecipeLiveVisible { get; set; } = "Visible";
    public static string btnRecipeProgressVisible { get; set; } = "Hidden";
    public static BindableCollection<RecipeControlData>? RecipeLiveData { get; set; }
    public static BindableCollection<RecipeControlData>? RecipeProgressData { get; set; }
    public List<RecipeRun>? RunData { get; set; }
    public static List<Event>? Events { get; set; }
    public static BindableCollection<Event> EventData { get; set; } = [];

    public PlotModel RecipeLivePlot { get; set; }
    public PlotModel RecipeProgressPlot { get; set; }
    static bool initProgressChart = false;
    static DateTime startTime;
    public static int totalTimeSpan = 60;
    public static readonly int MaxSecondsToLiveShow = 60;

    public ICommand BtnRecipeLiveVisibleCommand => new RelayCommand(RecipeLiveVisible);
    public ICommand BtnRecipeProgressVisibleCommand => new RelayCommand(RecipeProgressVisible);
    public ICommand BtnPlotAnalogCommand => new RelayCommand(PlotAnalog);
    public ICommand BtnPlotTemperatureCommand => new RelayCommand(PlotTemperature);
    public ICommand BtnPlotPressureCommand => new RelayCommand(PlotPressure);
    public ICommand BtnPlotTotalFlowCommand => new RelayCommand(PlotTotalFlow);


    /// <summary>
    /// Chart visibility changes
    /// </summary>

    private void RecipeLiveVisible()
    {
      btnRecipeLiveVisible = "Visible";
      btnRecipeProgressVisible = "Collapsed";
      OnPropertyChanged(nameof(btnRecipeLiveVisible));
      OnPropertyChanged(nameof(btnRecipeProgressVisible));
    }

    private void RecipeProgressVisible()
    {
      btnRecipeLiveVisible = "Collapsed";
      btnRecipeProgressVisible = "Visible";
      OnPropertyChanged(nameof(btnRecipeLiveVisible));
      OnPropertyChanged(nameof(btnRecipeProgressVisible));
    }


    /// <summary>
    /// Chart visibility changes
    /// </summary>

    private void PlotAnalog() => btnPlotAnalog = !btnPlotAnalog;

    private void PlotTemperature() => btnPlotTemperature = !btnPlotTemperature;

    private void PlotPressure() => btnPlotPressure = !btnPlotPressure;

    private void PlotTotalFlow() => btnPlotTotalFlow = !btnPlotTotalFlow;

    public BottomViewModel()
    {
      
      LivePlotSetting();
      ProgressPlotSetting();
      EventLoad();    //  테스트용. 이전 log 파일에서 log 데이터 임의 추출하여 보여주는 함수.

      RecipeLiveData = [];
      RecipeLiveData.CollectionChanged += RecipeLiveData_CollectionChanged;
      RecipeProgressData = [];
      RecipeProgressData.CollectionChanged += RecipeProgressData_CollectionChanged;


    }

    public static void ChartStartSensing()
    {
      StartSensing();
    }

    public static void ChartPauseSensing()
    {
      PauseSensing();
    }

    public void LivePlotSetting()
    {
      RecipeLivePlot = new();
      RecipeLivePlot.Axes.Add(new DateTimeAxis
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

      RecipeLivePlot.Axes.Add(new LinearAxis
      {
        Title = "Data Value",
        Position = AxisPosition.Left,
        IsPanEnabled = true,
        IsZoomEnabled = true,
      });

      string[] lineTitle =
      [
        "Analog", "Temperature", "Pressure", "TotalFlow",
        "AnalogSet", "TemperatureSet", "PressureSet", "TotalFlowSet",
      ];
      OxyColor[] lineColor =
      [
        OxyColors.Yellow, OxyColors.Red, OxyColors.Green, OxyColors.SkyBlue,
        OxyColors.DarkRed, OxyColors.DarkCyan, OxyColors.DarkBlue, OxyColors.DarkGreen
      ];
      MarkerType[] lineMaker =
      [
        MarkerType.None, MarkerType.None, MarkerType.None, MarkerType.None,
        MarkerType.None, MarkerType.None, MarkerType.None, MarkerType.None,
      ];
      for (int i = 0; i < 8; i++)
      {
        RecipeLivePlot.Series.Add(new LineSeries()
        {
          Title = lineTitle[i],
          Color = lineColor[i],
          MarkerStroke = lineColor[i],
          StrokeThickness = 1,
          MarkerType = lineMaker[i],
          MarkerSize = 2,
        });
      }
    }

    public void ProgressPlotSetting()
    {
      RecipeProgressPlot = new();
      RecipeProgressPlot.Axes.Add(new DateTimeAxis
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

      RecipeProgressPlot.Axes.Add(new LinearAxis
      {
        Title = "Data Value",
        Position = AxisPosition.Left,
        IsPanEnabled = true,
        IsZoomEnabled = true,
      });

      string[] lineTitle =
      [
        "Analog", "Temperature", "Pressure", "TotalFlow",
        "AnalogSet", "TemperatureSet", "PressureSet", "TotalFlowSet",
      ];
      OxyColor[] lineColor =
      [
        OxyColors.Yellow, OxyColors.Red, OxyColors.Green, OxyColors.SkyBlue,
        OxyColors.DarkRed, OxyColors.DarkCyan, OxyColors.DarkBlue, OxyColors.DarkGreen
      ];
      MarkerType[] lineMaker =
      [
        MarkerType.None, MarkerType.None, MarkerType.None, MarkerType.None,
        MarkerType.None, MarkerType.None, MarkerType.None, MarkerType.None,
      ];
      for (int i = 0; i < 8; i++)
      {
        RecipeProgressPlot.Series.Add(new LineSeries()
        {
          Title = lineTitle[i],
          Color = lineColor[i],
          MarkerStroke = lineColor[i],
          StrokeThickness = 1,
          MarkerType = lineMaker[i],
          MarkerSize = 2,
        });
      }

    }


    public void RecipeLiveData_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
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
      if (DateTimeAxis.ToDateTime(series1.Points.Last().X, TimeSpan.FromSeconds(MaxSecondsToLiveShow)) > DateTimeAxis.ToDateTime(dateTimeAxis.Maximum, TimeSpan.FromSeconds(MaxSecondsToLiveShow)))
      {
        dateTimeAxis.Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(-1 * MaxSecondsToLiveShow));
        dateTimeAxis.Maximum = DateTimeAxis.ToDouble(DateTime.Now);
        dateTimeAxis.Reset();
      }
      series1.IsVisible = btnPlotAnalog;
      series2.IsVisible = btnPlotTemperature;
      series3.IsVisible = btnPlotPressure;
      series4.IsVisible = btnPlotTotalFlow;

      RecipeLivePlot.InvalidatePlot(true);

      OnPropertyChanged(nameof(RecipeLivePlot));
    }

    public void RecipeProgressData_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems is null) return;
      var dateTimeAxis = RecipeProgressPlot.Axes.OfType<DateTimeAxis>().First();
      var series1 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(0);
      var series2 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(1);
      var series3 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(2);
      var series4 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(3);
      var series5 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(4);
      var series6 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(5);
      var series7 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(6);
      var series8 = RecipeProgressPlot.Series.OfType<LineSeries>().ElementAt(7);

      if (!initProgressChart)
      {
        startTime = DateTime.Now;
        var Recipeline = GetRecipeSetData();
        totalTimeSpan = Recipeline[2].Count;

        dateTimeAxis.Minimum = DateTimeAxis.ToDouble(startTime);
        dateTimeAxis.Maximum = DateTimeAxis.ToDouble(startTime.AddSeconds(totalTimeSpan));

        for (int i = 0; i < totalTimeSpan; i++)
        {
          series1.Points.Add(new DataPoint(DateTimeAxis.ToDouble(startTime.AddSeconds(i)), 0));
          series2.Points.Add(new DataPoint(DateTimeAxis.ToDouble(startTime.AddSeconds(i)), Recipeline[0][i]));
          series3.Points.Add(new DataPoint(DateTimeAxis.ToDouble(startTime.AddSeconds(i)), Recipeline[1][i]));
          series4.Points.Add(new DataPoint(DateTimeAxis.ToDouble(startTime.AddSeconds(i)), 0));
        }
        initProgressChart = true;
      }


      dateTimeAxis.Minimum = DateTimeAxis.ToDouble(startTime);
      dateTimeAxis.Maximum = DateTimeAxis.ToDouble(startTime.AddSeconds(totalTimeSpan));

      foreach (var newItem in e.NewItems)
      {
        if (newItem is RecipeControlData liveData)
        {
          series5.Points.Add(new DataPoint(DateTimeAxis.ToDouble(liveData.TimeStamp), liveData.Data1));
          series6.Points.Add(new DataPoint(DateTimeAxis.ToDouble(liveData.TimeStamp), liveData.Data2));
          series7.Points.Add(new DataPoint(DateTimeAxis.ToDouble(liveData.TimeStamp), liveData.Data3));
          series8.Points.Add(new DataPoint(DateTimeAxis.ToDouble(liveData.TimeStamp), liveData.Data4));
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

      RecipeProgressPlot.InvalidatePlot(true);
      OnPropertyChanged(nameof(RecipeProgressPlot));
    }


    // 챠트 Test 용 
    public static List<List<double>> GetRecipeSetData()
    {
      List<List<double>> YData = [[0], [0], [0], [0]];
      double[] y_init = [0, 0, 0, 0];
      double[] slope = [0, 0, 0, 0];
      List<double> y = [0, 0, 0, 0];

      string recipeCsvFile = RecipeViewModel.RecipeCsvFile;
      //string Filepath = OpenCsv;
      if (!string.IsNullOrWhiteSpace(recipeCsvFile) || !File.Exists(recipeCsvFile))
      {
        using var streamReader = new StreamReader(recipeCsvFile);
        using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
        csvReader.Read();
        while (csvReader.Read())
        {
          Recipe r = new()
          {
            rTime = csvReader.GetField<short>(2),
            hTime = csvReader.GetField<short>(3),
            sTemp = csvReader.GetField<short>(4),
            rPress = csvReader.GetField<short>(5),
          };

          slope[0] = (r.sTemp - y_init[0]) / r.rTime;
          slope[1] = (r.rPress - y_init[1]) / r.rTime;
          slope[2] = (r.sRotation - y_init[2]) / r.rTime;
          slope[3] = (r.cTemp - y_init[3]) / r.rTime;

          // Calculate elapse time for the recipe steps 
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

    public static void StartSensing()
    {
      if (_timer is null)
      {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
        _timer.Tick += SensorRecievedData;
        _timer.Tick += RisingEventData;
      }
      _timer.Start();
    }

    //public bool CanStopAcquisition => _timer?.IsEnabled == true;
    public static void PauseSensing()
    {
      _timer?.Stop();
    }



    public static void SensorRecievedData(object? sender, EventArgs e)
    {
      Random rand = new();
      try
      {
        uint rTempCV = MainViewModel.Ads.CreateVariableHandle("P11_E3508.rTemperatureControlValue");
        uint rPressCV = MainViewModel.Ads.CreateVariableHandle("P12_IQ_PLUS.rPressureControlValue_torr");
        uint rRotateCV = MainViewModel.Ads.CreateVariableHandle("P15_RotationOperation.rRotationSV_rpm");
        uint hDeviceArray = MainViewModel.Ads.CreateVariableHandle("Test_Recipe_Run.plc_array");
        tcTempCV = (double)MainViewModel.Ads.ReadAny(rTempCV, typeof(double));
        tcPressCV = (double)MainViewModel.Ads.ReadAny(rPressCV, typeof(double));
        tcRotateSV = (float)MainViewModel.Ads.ReadAny(rRotateCV, typeof(float));
        tcDeviceArray = (byte)MainViewModel.Ads.ReadAny(hDeviceArray, typeof(byte));
        Console.WriteLine($"{timerCount} : {tcDeviceArray}");



      }
      catch
      {
        tcTempCV = 0;
        tcPressCV = 0;
      }

      RecipeLiveData.Add(new()
      {
        TimeStamp = DateTime.Now,

        Data1 = 0,
        Data2 = tcTempCV,
        Data3 = tcPressCV,
        Data4 = 0,
      });

      RecipeProgressData.Add(new()
      {
        TimeStamp = DateTime.Now,

        Data1 = 0,
        Data2 = tcTempCV,
        Data3 = tcPressCV,
        Data4 = 0,
      });
      timerCount += 1;
    }


    /// <summary>
    /// 테스트용
    /// Datagrid에 이벤트 데이터 전달
    /// 이전 log 파일에서 log 데이터 임의 추출하여 보여주는 함수.
    /// </summary>
    public static void RisingEventData(object? sender, EventArgs e)
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


    /// <summary>
    /// 테스트용
    /// Datagrid에 이벤트 데이터 전달
    /// 이전 log 파일에서 log 데이터 로딩 함수.
    /// </summary>
    private void EventLoad()
    {
      Events = [];
      // load csv data and test 
      string Filename = "..\\..\\..\\..\\data\\eventLog\\_screen_datagrid_data.csv";
      _ = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ",", HasHeaderRecord = false };

      using var streamReader = new StreamReader(Filename);
      using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
      Events = csvReader.GetRecords<Event>().ToList();
    }




  }
}
