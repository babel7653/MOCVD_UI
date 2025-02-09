using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using SapphireXR_App.Common;
using SapphireXR_App.Models;

namespace SapphireXR_App.ViewModels.BottomDashBoard
{
    internal class HomeBottomDashBoardViewModel: BottomDashBoardViewModel
    {
        class ControlTargetValueSeriesUpdaterFromCurrentPLCState : BottomDashBoardViewModel.ControlTargetValueSeriesUpdater, IObserver<(int, int)>
        {
            internal ControlTargetValueSeriesUpdaterFromCurrentPLCState(string title) : base(title) { }

            protected override Axis initializeXAxis()
            {
                return new DateTimeAxis
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
                };
            }
            protected void update((int, int) value)
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

                series1.Points.RemoveAll((DataPoint dp) => dp.X < dateTimeAxis.Minimum);
                series2.Points.RemoveAll((DataPoint dp) => dp.X < dateTimeAxis.Minimum);

                dateTimeAxis.Reset();

                plotModel.InvalidatePlot(true);
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
                update(value);
            }

            public static readonly int MaxSecondsToLiveShow = 30;
        }

        public HomeBottomDashBoardViewModel(): base("CurrentPLCState")
        {
            seriesUpdaterSetting();
        }

        private void seriesUpdaterSetting()
        {
            foreach (var (id, index) in PLCService.dIndexController)
            {
                ControlTargetValueSeriesUpdaterFromCurrentPLCState controlCurrentValueSeriesUpdater = new ControlTargetValueSeriesUpdaterFromCurrentPLCState(id);
                ObservableManager<(int, int)>.Subscribe("FlowControl." + id + ".ControlTargetValue.CurrentPLCState", controlCurrentValueSeriesUpdater);
                plotModels[index] = controlCurrentValueSeriesUpdater;
            }
        }
    }
}
