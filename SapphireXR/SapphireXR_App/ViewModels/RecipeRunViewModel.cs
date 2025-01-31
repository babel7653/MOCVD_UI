using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Bases;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using SapphireXR_App.ViewModels.BottomDashBoard;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SapphireXR_App.ViewModels
{
    public partial class RecipeRunViewModel: ViewModelBase, IObserver<short>
    {
        public class LogIntervalInRecipeRunListener : IObserver<int>
        {
            public LogIntervalInRecipeRunListener(RecipeRunViewModel viewModel, int currentLogIntervalInRecipeRun) 
            {
                recipeRunViewModel = viewModel;
                currentValue = currentLogIntervalInRecipeRun;
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
                if (currentValue != value)
                {
                    recipeRunViewModel.resetLogTimer(value);
                    currentValue = value;
                }
            }
            RecipeRunViewModel recipeRunViewModel;
            int currentValue;
        }
           
        private static RecipeContext EmptyRecipeContext = new RecipeContext();

        [ObservableProperty]
        private RecipeContext _currentRecipe = EmptyRecipeContext;

        [ObservableProperty]
        string _startText = "Start";

        bool _recipeStarted = false;
        bool RecipeStarted
        {
            get { return _recipeStarted; }
            set { SetProperty(ref _recipeStarted, value); }
        }

        private static readonly CsvHelper.Configuration.CsvConfiguration Config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true
        };

        [RelayCommand]
        private void RecipeOpen()
        {
            (bool result, string? recipeFilePath, List<Recipe>? recipes) = RecipeService.OpenRecipe(Config);
            if (result == true)
            {
                if (0 < recipes!.Count)
                {
                    CurrentRecipe?.DisposeResource();
                    CurrentRecipe = new RecipeContext(recipeFilePath!, recipes!);
                }
                else
                {
                    MessageBox.Show(recipeFilePath + "은 빈 파일입니다.");
                }
            }
        }

      
        [RelayCommand(CanExecute =nameof(CurrentRecipeActive))]
        private void RecipeStart()
        {
            RecipeStarted = !RecipeStarted;
        }

        [RelayCommand(CanExecute = nameof(CurrentRecipeActive))]
        void RecipeSkip()
        {
            PLCService.WriteOperationState(60);
        }

        [RelayCommand(CanExecute = nameof(CurrentRecipeActive))]
        void RecipeRefresh()
        {
            CurrentRecipe.loadPLCSubRangeOfRecipes();
        }

        [RelayCommand]
        void ValveDataGridLoaded(object? args)
        {
            valveDataGrid ??= (args as RoutedEventArgs)?.Source as DataGrid;
        }

        [RelayCommand]
        void FlowDataGridLoaded(object? args)
        {
            flowDataGrid ??= (args as RoutedEventArgs)?.Source as DataGrid;
        }

        [RelayCommand]
        void ReactorDataGridLoaded(object? args)
        {
            reactorDataGrid ??= (args as RoutedEventArgs)?.Source as DataGrid;
        }

        [RelayCommand(CanExecute = nameof(CurrentRecipeActive))]
        private void RecipeStop()
        {
            PLCService.WriteStart(false);
            PLCService.WriteOperationState(40);
        }

        [RelayCommand]
        private void CheckUpperModifedRow(object? args)
        {
            DataGridRowEditEndingEventArgs? dataGridRowEditEndingEventArgs = args as DataGridRowEditEndingEventArgs;
            if(dataGridRowEditEndingEventArgs != null && dataGridRowEditEndingEventArgs.EditAction == DataGridEditAction.Commit)
            {
                Recipe? modified = dataGridRowEditEndingEventArgs.Row.DataContext as Recipe;
                if (modified != null)
                {
                    CurrentRecipe.addModfiedRecipe(modified);
                }
            }
        }

        public RecipeRunViewModel()
        {
            DashBoardViewModel = new RecipeRunBottomDashBoardViewModel();
            logIntervalInRecipeRunListener = new LogIntervalInRecipeRunListener(this, GlobalSetting.LogIntervalInRecipeRunInMS);
            ObservableManager<int>.Subscribe("GlobalSetting.LogIntervalInRecipeRun", logIntervalInRecipeRunListener);
            ObservableManager<short>.Subscribe("RecipeRun.CurrentActiveRecipe", this);

            logTimer = new DispatcherTimer();
            logTimer.Interval = new TimeSpan(TimeSpan.TicksPerMillisecond * GlobalSetting.LogIntervalInRecipeRunInMS);
            logTimer.Tick += (object? sender, EventArgs args) =>
            {
                CurrentRecipe.log();
            };
            logTimer.Start();

            PropertyChanged += (object? sender, PropertyChangedEventArgs e) =>
            {
                var recipeStart = () =>
                {
                    CurrentRecipe.onStart();
                };
                var recipePause = () => 
                {
                    PLCService.WriteOperationState(20);
                }; 
                switch (e.PropertyName)
                {
                    case nameof(RecipeStarted):
                        if(RecipeStarted == true)
                        {
                            recipeStart();
                            StartText = "Pause";

                        }
                        else
                        {
                            recipePause();
                            StartText = "Start";
                        }
                        break;

                    case nameof(CurrentRecipe):
                        RecipeStartCommand.NotifyCanExecuteChanged();
                        RecipeStopCommand.NotifyCanExecuteChanged();
                        RecipeRefreshCommand.NotifyCanExecuteChanged();
                        RecipeSkipCommand.NotifyCanExecuteChanged();
                        DashBoardViewModel.resetFlowChart(CurrentRecipe.Recipes);
                        break;
                }
            };
        }

        bool CurrentRecipeActive()
        {
            return CurrentRecipe != EmptyRecipeContext && CurrentRecipe != null;
        }

        void IObserver<short>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<short>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<short>.OnNext(short value)
        {
            Recipe? currentRecipe = CurrentRecipe.markCurrent(value);
            if (currentRecipe != null)
            {
                scrollIntoViewCurrentRecipe(reactorDataGrid, currentRecipe);
                scrollIntoViewCurrentRecipe(flowDataGrid, currentRecipe);
                scrollIntoViewCurrentRecipe(valveDataGrid, currentRecipe);
            }
        }

        private static void scrollIntoViewCurrentRecipe(DataGrid? dataGrid, Recipe recipe)
        {
            dataGrid?.Dispatcher.InvokeAsync(() =>
            {
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(recipe);
            });
        }

        public void resetLogTimer(int intervalInMS)
        {
            logTimer.Stop();
            logTimer.Interval = new TimeSpan(intervalInMS * TimeSpan.TicksPerMillisecond);
            logTimer.Start();
        }

        public RecipeRunBottomDashBoardViewModel DashBoardViewModel { get; set; }

        private DataGrid? reactorDataGrid;
        private DataGrid? flowDataGrid;
        private DataGrid? valveDataGrid;

        private LogIntervalInRecipeRunListener logIntervalInRecipeRunListener;
        private DispatcherTimer logTimer;
    }
}
