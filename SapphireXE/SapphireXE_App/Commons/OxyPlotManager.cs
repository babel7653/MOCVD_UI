using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace SapphireXE_App.Commons
{
  class OxyPlotManager
  {
    private IList<OxyColor>? _oxyColors;
    private int MaxSecondsToShow = 20;
    private int _oxyColorIndex;
    private readonly PlotModel _plotModel = new();
    public PlotModel PlotModel => _plotModel;

    public OxyPlotManager()
    {
    }

    public void SetTitle(string title)
    {
      PlotModel.Title = title;
    }

    /// <summary>
    /// X축 설정
    /// </summary>
    public void SetDateTiemAxisX(string title, string stringFormat)
    {
      PlotModel.Axes.Add(new DateTimeAxis
      {
        Position = AxisPosition.Bottom,
        Title = title,
        IntervalLength = 60,
        IntervalType = DateTimeIntervalType.Seconds,
        StringFormat = stringFormat,
        MajorGridlineStyle = LineStyle.Solid,
        MinorGridlineStyle = LineStyle.Solid,
        //Minimum = DateTimeAxis.ToDouble(DateTime.Now),
        //Maximum = DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(MaxSecondsToShow)),
        IsPanEnabled = true,
        IsZoomEnabled = true,
      });
    }

    public void SetAxisX(string title)
    {
      PlotModel.Axes.Add(new LinearAxis
      {
        Position = AxisPosition.Bottom,
        MajorGridlineStyle = LineStyle.Solid,
      });
    }

    /// <summary>
    /// Y축 설정
    /// </summary>
    public void SetAxisY(string title)
    {
      PlotModel.Axes.Add(new LinearAxis
      {
        Position = AxisPosition.Left,
        Title = title,
        IsPanEnabled = true,
        IsZoomEnabled = true,
        //Minimum = -2,
        //Maximum = 12,
      });
    }


    /// <summary>
    /// Legend 설정
    /// </summary>
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

    /// <summary>
    /// Color 설정
    /// </summary>
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
        StrokeThickness = 2,
        MarkerType = MarkerType.Circle,
        MarkerSize = 2,
      };
      lineSeries.Points.AddRange(dataPoints);
      PlotModel.Series.Add(lineSeries);
    }

    public void AddSeries(int num, string[] title, OxyColor[] color, MarkerType[] marker)
    {
      for (int i = 0; i < num; i++)
      {
        LineSeries lineSeries = new LineSeries
        {
          Title = title[i],
          Color = color[i],
          MarkerStroke = color[i],
          StrokeThickness = 1,
          MarkerType = marker[i],
          MarkerSize = 2,
        };
        PlotModel.Series.Add(lineSeries);
      }
    }


    /// <summary>
    /// Zoom 설정
    /// </summary>

    public void ResetAllAxes()
    {
      PlotModel.ResetAllAxes();
    }

    public void ZoomAllAxes(double factor)
    {
      double width = PlotModel.Width;
      double height = PlotModel.Height;
      PlotModel.ZoomAllAxes(factor);
    }


  }
}
