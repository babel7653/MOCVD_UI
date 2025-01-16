using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Caliburn.Micro;
using CommunityToolkit.Mvvm.Input;
using CsvHelper.Configuration;
using CsvHelper;
using SapphireXR_App.Bases;
using SapphireXR_App.Models;
using System.Globalization;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using SapphireXR_App.Common;
using System.Windows;

namespace SapphireXR_App.ViewModels
{
    public partial class BottomViewModel : ObservableObject
    {
        class ControlTargetValueSeriesUpdater : IObserver<(int, int)>
        {
            public ControlTargetValueSeriesUpdater(string title)
            {
                plotModel.Title = title;
                plotModel.TitleFontSize = plotModel.TitleFontSize / 2;
                plotModel.Axes.Add(new DateTimeAxis
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

                plotModel.Axes.Add(new LinearAxis
                {
                    Title = "Data Value",
                    Position = AxisPosition.Left,
                    IsPanEnabled = true,
                    IsZoomEnabled = true,
                });

                plotModel.Series.Add(new LineSeries()
                {
                    Title = "PV",
                    Color = OxyColors.Green,
                    MarkerStroke = OxyColors.Yellow,
                    StrokeThickness = 1,
                    MarkerType = MarkerType.None,
                    MarkerSize = 2,
                });
                plotModel.Series.Add(new LineSeries()
                {
                    Title = "SV",
                    Color = OxyColors.Red,
                    MarkerStroke = OxyColors.Red,
                    StrokeThickness = 1,
                    MarkerType = MarkerType.None,
                    MarkerSize = 2,
                });
            }

            void IObserver<(int, int)>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<(int, int)>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<(int, int)>.OnNext((int, int) value)
            {
                var dateTimeAxis = plotModel.Axes.OfType<DateTimeAxis>().First();
                var series1 = plotModel.Series.OfType<LineSeries>().ElementAt(0);
                var series2 = plotModel.Series.OfType<LineSeries>().ElementAt(1);

                if (!series1.Points.Any())
                {
                    dateTimeAxis.Minimum = DateTimeAxis.ToDouble(DateTime.Now);
                    dateTimeAxis.Maximum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(MaxSecondsToLiveShow));
                }

                double x = DateTimeAxis.ToDouble(DateTime.Now);
                series1.Points.Add(new DataPoint(x, (double)value.Item1));
                series2.Points.Add(new DataPoint(x, (double)value.Item2));

             
                dateTimeAxis.Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(-1 * MaxSecondsToLiveShow));
                dateTimeAxis.Maximum = DateTimeAxis.ToDouble(DateTime.Now);
                dateTimeAxis.Reset();
                

                plotModel.InvalidatePlot(true);
            }

            public void toggleControlValueSeries()
            {
                plotModel.Series.OfType<LineSeries>().ElementAt(0).IsVisible = !plotModel.Series.OfType<LineSeries>().ElementAt(0).IsVisible;
            }

            public void toggleTargetValueSeries()
            {
                plotModel.Series.OfType<LineSeries>().ElementAt(1).IsVisible = !plotModel.Series.OfType<LineSeries>().ElementAt(1).IsVisible;
            }

            public PlotModel plotModel = new PlotModel();
        }

        class SelectedFlowControllerListener : IObserver<string>
        {
            public SelectedFlowControllerListener(BottomViewModel vm)
            {
                bottomViewModel = vm;
            }
            void IObserver<string>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<string>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<string>.OnNext(string value)
            {
                ControlTargetValueSeriesUpdater currentSelectedFlowControllerListener = bottomViewModel.plotModels[PLCService.dIndexController[value]];
                bottomViewModel.currentSelectedFlowControllerListener = currentSelectedFlowControllerListener;
                bottomViewModel.FlowControlLivePlot = currentSelectedFlowControllerListener.plotModel;
            }

            private BottomViewModel bottomViewModel;
        }

        [ObservableProperty]
        public PlotModel? flowControlLivePlot;
        private ControlTargetValueSeriesUpdater[] plotModels = new ControlTargetValueSeriesUpdater[PLCService.NumControllers];
        private ControlTargetValueSeriesUpdater? currentSelectedFlowControllerListener;

        public BottomViewModel()
        {
            FlowControlLivePlot = new PlotModel();
            LivePlotSetting();
            ObservableManager<string>.Subscribe("FlowControl.Selected", flowContorllerSelectionChanged = new SelectedFlowControllerListener(this));
            ControlValueOption = HideControlValueStr;
            TargetValueOption = HideTargetValueStr;
        }
   
        public void LivePlotSetting()
        {
            foreach(var (id, index) in PLCService.dIndexController)
            {
                ControlTargetValueSeriesUpdater controlCurrentValueSeriesUpdater = new ControlTargetValueSeriesUpdater(id);
                ObservableManager<(int, int)>.Subscribe("FlowControl." + id + ".ControlTargetValue", controlCurrentValueSeriesUpdater);
                plotModels[index] = controlCurrentValueSeriesUpdater;
            }
        }

        public ICommand ShowControlValue => new RelayCommand(() =>
        {
            if (currentSelectedFlowControllerListener != null)
            {
                currentSelectedFlowControllerListener.toggleControlValueSeries();
                if (ControlValueOption == ShowControlValueStr)
                {
                    ControlValueOption = HideControlValueStr;
                }
                else
                    if (ControlValueOption == HideControlValueStr)
                    {
                        ControlValueOption = ShowControlValueStr;
                    }
            }
        });
        public ICommand ShowTargetValue => new RelayCommand(() =>
        {
            if (currentSelectedFlowControllerListener != null)
            {
                currentSelectedFlowControllerListener?.toggleTargetValueSeries();
                if (TargetValueOption == ShowTargetValueStr)
                {
                    TargetValueOption = HideTargetValueStr;
                }
                else
                    if (TargetValueOption == HideTargetValueStr)
                    {
                        TargetValueOption = ShowTargetValueStr;
                    }
            }
        });

        private IObserver<string> flowContorllerSelectionChanged;
        public static readonly int MaxSecondsToLiveShow = 30;

        [ObservableProperty]
        public string controlValueOption;
        [ObservableProperty]
        public string targetValueOption;

        private static string ShowControlValueStr = "Show Control Value";
        private static string HideControlValueStr = "Hide Control Value";
        private static string ShowTargetValueStr = "Show Target Value";
        private static string HideTargetValueStr = "Hide Target Value";
    }
}
