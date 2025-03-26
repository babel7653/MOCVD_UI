using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using SapphireXR_App.Models;
using System.Globalization;
using System.IO;
using System.Windows;
using System.ComponentModel;
using SapphireXR_App.Common;
using OxyPlot.Annotations;

namespace SapphireXR_App.ViewModels
{
    public partial class ReportViewModel : ObservableObject, IObserver<(ReportSeriesSelectionViewModel.SelectionToShowChanged, IList<string>)>
    {
        public enum LogNumber { One = 0, Two };
        public enum Mode { DataValue, Percentage };

        private struct PlotModelByType
        {
            public PlotModelByType() { }

            internal readonly Dictionary<string, LineSeries> logSeries = new Dictionary<string, LineSeries>();
            internal readonly PlotModel plotModel = new PlotModel();
        };

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
            dataValuePlotModel.plotModel.Axes.Add(new LinearAxis
            {
                Title = "Data Value",
                Position = AxisPosition.Left,
                IsPanEnabled = true,
                IsZoomEnabled = true
            });
            dataValuePlotModel.plotModel.Axes.Add(new TimeSpanAxis
            {
                Title = "Time Stamp",
                Position = AxisPosition.Bottom,
                IntervalLength = 60,
                IsPanEnabled = true,
                IsZoomEnabled = true,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
            });
            dataValuePlotModel.plotModel.Legends.Add(new Legend() { Key= "CurrentTargetValue" });

            percentagePlotModel.plotModel.Axes.Add(new LinearAxis
            {
                Title = "Percentage (%)",
                Position = AxisPosition.Left,
                IntervalLength=10,
                IsPanEnabled = true,
                IsZoomEnabled = true
            });
            percentagePlotModel.plotModel.Axes.Add(new TimeSpanAxis
            {
                Title = "Time Stamp",
                Position = AxisPosition.Bottom,
                IntervalLength = 60,
                IsPanEnabled = true,
                IsZoomEnabled = true,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
            });
            percentagePlotModel.plotModel.Legends.Add(new Legend() { Key = "CurrentTargetValue" });
            FlowControlLivePlot = dataValuePlotModel.plotModel;

            PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
            {
                switch(args.PropertyName)
                {
                    case nameof(CurrentMode):
                        switch(CurrentMode)
                        {
                            case Mode.DataValue:
                                FlowControlLivePlot = dataValuePlotModel.plotModel;
                                break;

                            case Mode.Percentage:
                                FlowControlLivePlot = percentagePlotModel.plotModel;
                                break;
                        }
                        break;
                }
            };
            ObservableManager<(ReportSeriesSelectionViewModel.SelectionToShowChanged, IList<string>)>.Subscribe("ReportSeriesSelection.ToShowChanged", this);
        }

        public void updateLogSeries(LogNumber logNumber, IList<RecipeLog> recipeLogs)
        {
            string prefix = titlePrefixes[(int)logNumber];
            var doUpdateLogSeries = (PlotModelByType plotModelByType, Mode mode) =>
            {
                DateTime? firstTime = null;
                foreach (string name in LogReportSeries.LogSeriesColor.Keys)
                {
                    string seriesTitle = prefix + name;
                    LineSeries? seriesForDevice = null;
                    if (plotModelByType.logSeries.TryGetValue(seriesTitle, out seriesForDevice) == false)
                    {
                        seriesForDevice = new LineSeries()
                        {
                            Title = seriesTitle,
                            MarkerStroke = OxyColors.Black,
                            StrokeThickness = 1,
                            MarkerType = MarkerType.None,
                            MarkerSize = 2,
                            LegendKey = FlowControlLivePlot.Legends[0].Key,
                            IsVisible = true
                        };
                        plotModelByType.logSeries[seriesTitle] = seriesForDevice;
                        if(reportSeriesSelectionViewModel != null)
                        {
                            if(reportSeriesSelectionViewModel.SelectedNames.FirstOrDefault(selectName => name == selectName) != default)
                            {
                                plotModelByType.plotModel.Series.Add(seriesForDevice);
                            }
                        }    
                    }
                    seriesForDevice!.Points.Clear();
                    foreach (RecipeLog recipeLog in recipeLogs)
                    {
                        if (firstTime == null)
                        {
                            firstTime = recipeLog.LogTime;
                        }
                        float? value = mode == Mode.DataValue ? LogReportSeries.GetFlowValue(recipeLog, name) : LogReportSeries.GetFlowPercentageValue(recipeLog, name);
                        if (value != null)
                        {
                            seriesForDevice.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(recipeLog.LogTime - firstTime), (double)value));
                        }
                    }
                }
                zoomToFit(mode, plotModelByType);
                plotModelByType.plotModel.InvalidatePlot(true);
            };

            doUpdateLogSeries(dataValuePlotModel, Mode.DataValue);
            doUpdateLogSeries(percentagePlotModel, Mode.Percentage);
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
            string filePath = updateChart(LogNumber.One);
            Log1FilePath = filePath != string.Empty ? filePath : Log1FilePath;
        }

        [RelayCommand]
        public void OpenLog2File()
        {
            string? filePath = updateChart(LogNumber.Two);
            Log2FilePath = filePath != string.Empty ? filePath : Log2FilePath;
        }

        [RelayCommand]
        public void ShowSeriesSelectionView()
        {
            if(reportSeriesSelectionViewModel == null)
            {
                reportSeriesSelectionViewModel = new ReportSeriesSelectionViewModel();
            }
            ReportSeriesSelectionEx.Show(reportSeriesSelectionViewModel);
        }

        [RelayCommand]
        private void Export()
        {
            SaveFileDialog saveFileDialog = new() { Filter = "svg 파일(*.svg)|*.svg", InitialDirectory = AppDomain.CurrentDomain.BaseDirectory };
            if(saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (FileStream fileStream = File.Create(saveFileDialog.FileName))
                    {
                        new SvgExporter { Width = FlowControlLivePlot.Width, Height = FlowControlLivePlot.Height }.Export(FlowControlLivePlot, fileStream);
                    }
                }
                catch(Exception exception)
                {
                    MessageBox.Show("SVG 파일로 내보내는데 실패햇습니다. 원인은 다음과 같습니다: " + exception.Message);
                }
            }
        }

        private void zoomToFit(Mode mode, PlotModelByType plotModelByType)
        {
            if (plotModelByType.logSeries.Count <= 0)
            {
                return;
            }
            if (mode == Mode.DataValue)
            {
                
                plotModelByType.plotModel.Axes[0].Zoom(-10.0, plotModelByType.logSeries.Max((keyValuePair => 0 < (keyValuePair.Value).Points.Count ? (keyValuePair.Value).Points.Max((dataPoint => dataPoint.Y)) : 0)) + 10.0);
            }
            else
            {
                plotModelByType.plotModel.Axes[0].Zoom(-0.1, 100.1);
            }
            plotModelByType.plotModel.Axes[1].Zoom(0.0, plotModelByType.logSeries.Max(keyValuePair => 0 < (keyValuePair.Value).Points.Count ? (keyValuePair.Value).Points.Max((dataPoint => dataPoint.X)) : 0));
        }

        [RelayCommand]
        private void ZoomToFit()
        {
            if (FlowControlLivePlot == dataValuePlotModel.plotModel)
            {
                zoomToFit(CurrentMode, dataValuePlotModel);
                dataValuePlotModel.plotModel.InvalidatePlot(true);
            }
            else
            {
                zoomToFit(CurrentMode, percentagePlotModel);
                percentagePlotModel.plotModel.InvalidatePlot(true);
            }
        }

        private void setVisibleForGivenList(IList<string> givenList, bool visible)
        {
            var doSetVisibleForGivenList = (PlotModelByType plotModelByType) =>
            {
                foreach (string name in givenList)
                {
                    for (int log = 0; log < 2; ++log)
                    {
                        LineSeries? series = null;
                        if (plotModelByType.logSeries.TryGetValue(titlePrefixes[log] + name, out series) == true)
                        {
                            if (visible == true && plotModelByType.plotModel.Series.Contains(series) == false)
                            {
                                plotModelByType.plotModel.Series.Add(series);
                            }
                            else
                            {
                                plotModelByType.plotModel.Series.Remove(series);
                            }
                        }
                    }
                }
                plotModelByType.plotModel.InvalidatePlot(true);
            };
            doSetVisibleForGivenList(dataValuePlotModel);
            doSetVisibleForGivenList(percentagePlotModel);
        }

        void IObserver<(ReportSeriesSelectionViewModel.SelectionToShowChanged, IList<string>)>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<(ReportSeriesSelectionViewModel.SelectionToShowChanged, IList<string>)>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<(ReportSeriesSelectionViewModel.SelectionToShowChanged, IList<string>)>.OnNext((ReportSeriesSelectionViewModel.SelectionToShowChanged, IList<string>) value)
        {
            setVisibleForGivenList(value.Item2, value.Item1 == ReportSeriesSelectionViewModel.SelectionToShowChanged.Added ? true : false);
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

        private readonly string[] titlePrefixes = ["Log 1 ", "Log 2 "];


        private readonly PlotModelByType dataValuePlotModel = new PlotModelByType();
        private readonly PlotModelByType percentagePlotModel = new PlotModelByType();
        [ObservableProperty]
        public PlotModel _flowControlLivePlot;
        private ReportSeriesSelectionViewModel? reportSeriesSelectionViewModel = null;
        public IList<Mode> ChartMode { get; } = [Mode.DataValue, Mode.Percentage];
        [ObservableProperty]
        private Mode _currentMode = Mode.DataValue;
    }
}
