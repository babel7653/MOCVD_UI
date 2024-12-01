using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireXR_App.Common
{
    public class OxyPlotManager
    {
        private readonly PlotModel _plotModel = new PlotModel();
        private IList<OxyColor>? _oxyColors;
        private int _oxyColorIndex;

        public OxyPlotManager(string title)
        {
            _plotModel = new PlotModel { Title = title };
        }
        public PlotModel PlotModel => _plotModel;

        // X축 설정 - Time Base
        public void SetDateTimeAxisX(string title, string stringFormat)
        {
            PlotModel.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = title,
                IntervalLength = 60,
                IntervalType = DateTimeIntervalType.Minutes,
                StringFormat = stringFormat,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                IsPanEnabled = true,
                IsZoomEnabled = true,
            });
        }
        // X축 설정
        public void SetAxisX()
        {
            PlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Solid,
            });
        }
        // Y축 설정
        public void SetAxisY(string title)
        {
            PlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = title,
                IsPanEnabled = true,
                IsZoomEnabled = true,
            });
        }
        // Legend 설정
        public void SetRegend()
        {
            Legend legend = new Legend
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.RightTop,
                LegendOrientation = LegendOrientation.Vertical,
            };
            PlotModel.Legends.Add(legend);
        }
        // Color 설정
        public void SetOxyColors(int count)
        {
            _oxyColors = OxyPalettes.HueDistinct(count).Colors;
        }
        public void SetNextColor()
        {
            _oxyColorIndex = _oxyColors?.Count == _oxyColorIndex ? 0 : ++_oxyColorIndex;
        }
        public void AddLineSeriesDataPoints(string title, IEnumerable<DataPoint> dataPoints)
        {
            OxyColor color = _oxyColors == null ? OxyColors.Blue : _oxyColors[_oxyColorIndex];
            LineSeries lineSeries = new LineSeries
            {
                Title = title,
                Color = color,
                MarkerStroke = color,
                StrokeThickness = 1,
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
            };
            lineSeries.Points.AddRange(dataPoints);
            PlotModel.Series.Add(lineSeries);
        }
    }
}
