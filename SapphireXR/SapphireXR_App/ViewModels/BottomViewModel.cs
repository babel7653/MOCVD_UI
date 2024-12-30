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

namespace SapphireXR_App.ViewModels
{
    public class BottomViewModel : ObservableObject
    {
        public static BindableCollection<(int, int)>? RecipeLiveData { get; set; }
        public PlotModel RecipeLivePlot { get; set; } = new PlotModel();
        public static readonly int MaxSecondsToLiveShow = 60;

        public BottomViewModel()
        {
            LivePlotSetting();

            RecipeLiveData = [];
            RecipeLiveData.CollectionChanged += RecipeLiveData_CollectionChanged;
        }

   
        public void LivePlotSetting()
        {
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

            RecipeLivePlot.Series.Add(new LineSeries()
            {
                Title = "PV",
                Color = OxyColors.Yellow,
                MarkerStroke = OxyColors.Yellow,
                StrokeThickness = 1,
                MarkerType = MarkerType.None,
                MarkerSize = 2,
            });
            RecipeLivePlot.Series.Add(new LineSeries()
            {
                Title = "SV",
                Color = OxyColors.Red,
                MarkerStroke = OxyColors.Red,
                StrokeThickness = 1,
                MarkerType = MarkerType.None,
                MarkerSize = 2,
            });
        }

        public void RecipeLiveData_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is null) return;

            var dateTimeAxis = RecipeLivePlot.Axes.OfType<DateTimeAxis>().First();
            var series1 = RecipeLivePlot.Series.OfType<LineSeries>().ElementAt(0);
            var series2 = RecipeLivePlot.Series.OfType<LineSeries>().ElementAt(1);

            if (!series1.Points.Any())
            {
                dateTimeAxis.Minimum = DateTimeAxis.ToDouble(DateTime.Now);
                dateTimeAxis.Maximum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(MaxSecondsToLiveShow));
            }

            //foreach (var newItem in e.NewItems)
            //{
            //    if (newItem is (int, int) liveData)
            //    {
            //        series1.Points.Add(new DataPoint(DateTimeAxis.ToDouble(liveData.TimeStamp), liveData.Data1));
            //        series2.Points.Add(new DataPoint(DateTimeAxis.ToDouble(liveData.TimeStamp), liveData.Data2));
            //    }
            //}

            // if (series.Points.Last().X > dateTimeAxis.Maximum)
            if (DateTimeAxis.ToDateTime(series1.Points.Last().X, TimeSpan.FromSeconds(MaxSecondsToLiveShow)) > DateTimeAxis.ToDateTime(dateTimeAxis.Maximum, TimeSpan.FromSeconds(MaxSecondsToLiveShow)))
            {
                dateTimeAxis.Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(-1 * MaxSecondsToLiveShow));
                dateTimeAxis.Maximum = DateTimeAxis.ToDouble(DateTime.Now);
                dateTimeAxis.Reset();
            }

            RecipeLivePlot.InvalidatePlot(true);

            OnPropertyChanged(nameof(RecipeLivePlot));
        }
    }
}
