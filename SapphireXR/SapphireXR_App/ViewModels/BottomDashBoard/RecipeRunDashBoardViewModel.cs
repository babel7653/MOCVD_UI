using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SapphireXR_App.ViewModels.BottomDashBoardViewModel;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using System.Windows.Media;
using System.Reactive;

namespace SapphireXR_App.ViewModels.BottomDashBoard
{
    public class RecipeRunBottomDashBoardViewModel: BottomDashBoardViewModel
    {
        class ControlTargetValueSeriesUpdaterForRecipeRun : ControlTargetValueSeriesUpdater, IObserver<RecipeRunViewModel.RecipeUserState>, IObserver<int>
        {
            internal ControlTargetValueSeriesUpdaterForRecipeRun(string title) : base(title) 
            {
                ObservableManager<RecipeRunViewModel.RecipeUserState>.Subscribe("RecipeRun.State", this);
            }

            protected override Axis initializeXAxis()
            {
                return new TimeSpanAxis
                {
                    Title = "Time Span (Second)",
                    Position = AxisPosition.Bottom,
                    IntervalLength = 60
                };
            }

            private float GetFlowControllerValue(string flowControllerID, Recipe recipe)
            {
                switch (plotModel.Title)
                {
                    case "MFC01":
                        return recipe.M01;

                    case "MFC02":
                        return recipe.M02;

                    case "MFC03":
                        return recipe.M03;

                    case "MFC04":
                        return recipe.M04;

                    case "MFC05":
                        return recipe.M05;

                    case "MFC06":
                        return recipe.M06;

                    case "MFC07":
                        return recipe.M07;

                    case "MFC08":
                        return recipe.M08;

                    case "MFC09":
                        return recipe.M09;

                    case "MFC10":
                        return recipe.M10;

                    case "MFC11":
                        return recipe.M11;

                    case "MFC12":
                        return recipe.M12;

                    case "MFC13":
                        return recipe.M13;

                    case "MFC14":
                        return recipe.M14;

                    case "MFC15":
                        return recipe.M15;

                    case "MFC16":
                        return recipe.M04;

                    case "MFC17":
                        return recipe.M17;

                    case "MFC18":
                        return recipe.M18;

                    case "MFC19":
                        return recipe.M19;

                    case "EPC01":
                        return recipe.E01;

                    case "EPC02":
                        return recipe.E02;

                    case "EPC03":
                        return recipe.E03;

                    case "EPC04":
                        return recipe.E04;

                    case "EPC05":
                        return recipe.E05;

                    case "EPC06":
                        return recipe.E06;

                    case "EPC07":
                        return recipe.E07;

                    case "Temperature":
                        return recipe.STemp;

                    case "Pressure":
                        return recipe.RPress;

                    case "Rotation":
                        return recipe.SRotation;

                    default:
                        return 0.0f;
                }
            }

            public void initChart(IList<Recipe> recipes)
            {
                cleanChart();
                if (0 < recipes.Count)
                {
                    LegendUpdate = true;
                    uint accumTime = 0;
                    var series1 = plotModel.Series.OfType<LineSeries>().ElementAt(0);
                    series1.Points.Add(new DataPoint(accumTime, 0));
                    foreach (Recipe recipe in recipes)
                    {
                        float flowControllerValue = GetFlowControllerValue(plotModel.Title, recipe);
                        accumTime += (uint)recipe.RTime;
                        series1.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(TimeSpan.FromSeconds(accumTime)), flowControllerValue));
                        accumTime += (uint)recipe.HTime;
                        series1.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(TimeSpan.FromSeconds(accumTime)), flowControllerValue));
                    }
                    plotModel.Axes[0].Maximum = TimeSpanAxis.ToDouble(TimeSpan.FromSeconds(accumTime));
                }
            }

            protected void update(int value)
            {
                if(LegendUpdate == false)
                {
                    return;
                }
                var series2 = plotModel.Series.OfType<LineSeries>().ElementAt(1);

                DateTime now = DateTime.Now;
                TimeSpan timeSpan = (now - (firstTime ??= now));
                double x = TimeSpanAxis.ToDouble(timeSpan);
                series2.Points.Add(new DataPoint(x, (double)value));

                plotModel.InvalidatePlot(true);
            }

            private void cleanChart()
            {
                plotModel.Series.OfType<LineSeries>().ElementAt(0).Points.Clear();
                plotModel.Series.OfType<LineSeries>().ElementAt(1).Points.Clear();
                firstTime = null;
                LegendUpdate = false;
            }

            void IObserver<RecipeRunViewModel.RecipeUserState>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<RecipeRunViewModel.RecipeUserState>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<RecipeRunViewModel.RecipeUserState>.OnNext(RecipeRunViewModel.RecipeUserState recipeRunState)
            {
                LegendUpdate = recipeRunState == RecipeRunViewModel.RecipeUserState.Run;
            }

            void IObserver<int>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<int>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<int>.OnNext(int value)
            {
                update(value);
            }

            private DateTime? firstTime = null;
            public bool LegendUpdate { get; set; } = false;
        }

        public RecipeRunBottomDashBoardViewModel(): base("CurrentPLCState")
        {
            initSeriesUpdater();
        }

        private void initSeriesUpdater()
        {
            foreach (var (id, index) in PLCService.dIndexController)
            {
                ControlTargetValueSeriesUpdaterForRecipeRun controlCurrentValueSeriesUpdater = new ControlTargetValueSeriesUpdaterForRecipeRun(id);
                ObservableManager<int>.Subscribe("FlowControl." + id + ".ControlValue", controlCurrentValueSeriesUpdater);
                plotModels[index] = controlCurrentValueSeriesUpdater;
            }
        }

        public void resetFlowChart(IList<Recipe> recipes)
        {
            foreach (ControlTargetValueSeriesUpdaterForRecipeRun controlTargetValueSeriesUpdater in plotModels)
            {
                controlTargetValueSeriesUpdater.initChart(recipes);
            }
        }
    }
}
