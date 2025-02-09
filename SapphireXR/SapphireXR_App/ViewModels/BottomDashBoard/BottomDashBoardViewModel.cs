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
using OxyPlot.Legends;

namespace SapphireXR_App.ViewModels
{
    public partial class BottomDashBoardViewModel : ObservableObject
    {
        public abstract class ControlTargetValueSeriesUpdater
        {
            public ControlTargetValueSeriesUpdater(string title)
            {
                plotModel.Title = title;
                plotModel.TitleFontSize = plotModel.TitleFontSize / 2;
                plotModel.Axes.Add(initializeXAxis());

                plotModel.Axes.Add(new LinearAxis
                {
                    Title = "Data Value",
                    Position = AxisPosition.Left,
                    IsPanEnabled = true,
                    IsZoomEnabled = true,
                });

                Legend legend = new Legend();
                legend.Key = "ControlTargetValue";
                plotModel.Legends.Add(legend);

                plotModel.Series.Add(new LineSeries()
                {
                    Title = "PV",
                    Color = OxyColors.Green,
                    MarkerStroke = OxyColors.Yellow,
                    StrokeThickness = 1,
                    MarkerType = MarkerType.None,
                    MarkerSize = 2,
                    LegendKey = legend.Key
                });
                plotModel.Series.Add(new LineSeries()
                {
                    Title = "SV",
                    Color = OxyColors.Red,
                    MarkerStroke = OxyColors.Red,
                    StrokeThickness = 1,
                    MarkerType = MarkerType.None,
                    MarkerSize = 2,
                    LegendKey = legend.Key
                });
            }

            protected abstract Axis initializeXAxis();

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
            public SelectedFlowControllerListener(BottomDashBoardViewModel vm)
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

            private BottomDashBoardViewModel bottomViewModel;
        }

        [ObservableProperty]
        public PlotModel? flowControlLivePlot;
        protected ControlTargetValueSeriesUpdater[] plotModels = new ControlTargetValueSeriesUpdater[PLCService.NumControllers];
        private ControlTargetValueSeriesUpdater? currentSelectedFlowControllerListener;

        public BottomDashBoardViewModel(string flowControlSelectedPostFixStr)
        {
            FlowControlLivePlot = new PlotModel();
            ObservableManager<string>.Subscribe("FlowControl.Selected." + flowControlSelectedPostFixStr, flowContorllerSelectionChanged = new SelectedFlowControllerListener(this));
            ControlValueOption = HideControlValueStr;
            TargetValueOption = HideTargetValueStr;
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

        private readonly IObserver<string> flowContorllerSelectionChanged;

        [ObservableProperty]
        public string controlValueOption;
        [ObservableProperty]
        public string targetValueOption;

        private static readonly string ShowControlValueStr = "Show Control Value";
        private static readonly string HideControlValueStr = "Hide Control Value";
        private static readonly string ShowTargetValueStr = "Show Target Value";
        private static readonly string HideTargetValueStr = "Hide Target Value";
    }
}
