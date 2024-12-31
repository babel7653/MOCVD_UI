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

namespace SapphireXR_App.ViewModels
{
    public partial class BottomViewModel : ObservableObject
    {
        class ControlCurrentValueSeriesUpdater : IObserver<(int, int)>
        {
            public ControlCurrentValueSeriesUpdater()
            {
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
                double x = DateTimeAxis.ToDouble(DateTime.Now);
                series1.Points.Add(new DataPoint(x, (double)value.Item1));
                series2.Points.Add(new DataPoint(x, (double)value.Item2));

                plotModel.InvalidatePlot(true);
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
                bottomViewModel.FlowControlLivePlot = bottomViewModel.plotModels[PLCService.dIndexController[value]].plotModel;
            }

            private BottomViewModel bottomViewModel;
        }

        [ObservableProperty]
        public PlotModel flowControlLivePlot;

        private ControlCurrentValueSeriesUpdater[] plotModels = new ControlCurrentValueSeriesUpdater[PLCService.dIndexController.Count];

        public BottomViewModel()
        {
            FlowControlLivePlot = new PlotModel();
            LivePlotSetting();
            ObservableManager<string>.Subscribe("FlowControl.Selected", flowContorllerSelectionChanged = new SelectedFlowControllerListener(this));
        }
   
        public void LivePlotSetting()
        {
            foreach(var (id, index) in PLCService.dIndexController)
            {
                ControlCurrentValueSeriesUpdater controlCurrentValueSeriesUpdater = new ControlCurrentValueSeriesUpdater();
                ObservableManager<(int, int)>.Subscribe("FlowControl." + id + ".ControlTargetValue", controlCurrentValueSeriesUpdater);
                plotModels[index] = controlCurrentValueSeriesUpdater;
            }
        }

        private IObserver<string> flowContorllerSelectionChanged;
    }
}
