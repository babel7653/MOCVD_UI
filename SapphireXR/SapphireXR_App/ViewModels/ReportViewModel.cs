using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using SapphireXR_App.Models;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Collections;

namespace SapphireXR_App.ViewModels
{
    public partial class ReportViewModel : ObservableObject
    {
        public enum LogNumber { One, Two };

        private static (IList<RecipeLog>?, string) OpenLogFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "csv 파일(*.csv)|*.csv";
            string appBasePath = AppDomain.CurrentDomain.BaseDirectory;

            if (openFileDialog.ShowDialog() == false)
            {
                return (null, string.Empty);
            }

            try
            {
                using (FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open))
                {
                    using (StreamReader streamReder = new StreamReader(fileStream))
                    {
                        using (CsvReader csvReader = new CsvReader(streamReder, Config))
                        {
                            return (csvReader.GetRecords<RecipeLog>().ToList(), openFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(openFileDialog.FileName + "를 여는데 실패하였습니다. 올바른 로그 파일이 아닐 수 있습니다. 자세한 오류는 다음과 같습니다: " + exception.Message);
            }

            return (null, string.Empty);
        }

        public ReportViewModel() 
        {
            FlowControlLivePlot.Axes.Add(new LinearAxis
            {
                Title = "Data Value",
                Position = AxisPosition.Left,
                IsPanEnabled = true,
                IsZoomEnabled = true
            });
            FlowControlLivePlot.Axes.Add(new TimeSpanAxis
            {
                Title = "Time Stamp",
                Position = AxisPosition.Bottom,
                IntervalLength = 60,
                IsPanEnabled = true,
                IsZoomEnabled = true,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
            });
            FlowControlLivePlot.Legends.Add(new Legend() { Key= "CurrentTargetValue" });
        }

        public void updateLogSeries(LogNumber logNumber, IList<RecipeLog> recipeLogs)
        {
            string prefix = logNumber == LogNumber.One ? "Log 1 " : "Log 2 ";
            DateTime? firstTime = null;
            foreach (string name in LogReportSeries.LogSeriesColor.Keys)
            {
                string seriesTitle = prefix + name;
                Series? seriesForDevice = FlowControlLivePlot.Series.FirstOrDefault(element => element.Title == seriesTitle);
                if (seriesForDevice == default)
                {
                    seriesForDevice = new LineSeries()
                    {
                        Title = seriesTitle,
                        Color = logNumber == LogNumber.One ? LogReportSeries.LogSeriesColor[name].Item1 : LogReportSeries.LogSeriesColor[name].Item2,
                        MarkerStroke = OxyColors.Black,
                        StrokeThickness = 1,
                        MarkerType = MarkerType.None,
                        MarkerSize = 2,
                        LegendKey = FlowControlLivePlot.Legends[0].Key,
                        IsVisible = false
                    };
                    FlowControlLivePlot.Series.Add(seriesForDevice);
                }
                ((LineSeries)seriesForDevice).Points.Clear();
                foreach (RecipeLog recipeLog in recipeLogs)
                {
                    if (firstTime == null)
                    {
                        firstTime = recipeLog.LogTime;
                    }
                    float? flowValue = LogReportSeries.GetFlowValue(recipeLog, name);
                    if (flowValue != null)
                    {
                        ((LineSeries)seriesForDevice).Points.Add(new DataPoint(TimeSpanAxis.ToDouble(recipeLog.LogTime - firstTime), (double)flowValue));
                    }
                }
            }
            FlowControlLivePlot.Axes[0].Zoom(-10.0, FlowControlLivePlot.Series.Max(series => ((LineSeries)series).Points.Max(dataPoint => dataPoint.Y)) + 10.0);
            FlowControlLivePlot.InvalidatePlot(true);

        }

        private string updateChart(LogNumber logNumber)
        {
            (IList<RecipeLog>? recipeLogs, string? logFilePath) = OpenLogFile();
            if(recipeLogs != null)
            {
                updateLogSeries(logNumber, recipeLogs);
            }

            return logFilePath;
        }

        [RelayCommand]
        public void OpenLog1File()
        {
            Log1FilePath = updateChart(LogNumber.One);
        }

        [RelayCommand]
        public void OpenLog2File()
        {
            Log2FilePath = updateChart(LogNumber.Two);
        }

        [RelayCommand]
        public void ShowSeriesSelectionView()
        {
            if(reportSeriesSelectionViewModel == null)
            {
                reportSeriesSelectionViewModel = new ReportSeriesSelectionViewModel();
                reportSeriesSelectionViewModel.SelectedNames.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs args) =>
                {
                    var setVisibleForGivenList = (IList givenList, bool visible) =>
                    {
                        foreach (object item in givenList)
                        {
                            string? name = item as string;
                            if (name != null)
                            {
                                foreach (Series series in FlowControlLivePlot.Series.Where((Series series) => series.Title.Contains(name)))
                                {
                                    series.IsVisible = visible;
                                }
                            }
                        }
                    };
                    switch(args.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            if (args.NewItems != null)
                            {
                                setVisibleForGivenList(args.NewItems, true);
                            }
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            if(args.OldItems != null)
                            {
                                setVisibleForGivenList(args.OldItems, false);
                            }
                            break;
                    }
                };
            }
            ReportSeriesSelectionEx.Show(reportSeriesSelectionViewModel);
        }

        private static readonly CsvHelper.Configuration.CsvConfiguration Config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true
        };

        [ObservableProperty]
        private string _log1FilePath = "";

        [ObservableProperty]
        private string _log2FilePath = "";

        public PlotModel FlowControlLivePlot { get; set; } = new PlotModel();
        private ReportSeriesSelectionViewModel? reportSeriesSelectionViewModel = null;
    }
}
