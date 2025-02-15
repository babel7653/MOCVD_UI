﻿using Caliburn.Micro;
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
using static SapphireXR_App.Common.RecipeService;
using static SapphireXR_App.ViewModels.RecipeEditViewModel.TabDataGridViewModel;

namespace SapphireXR_App.ViewModels
{
    public partial class RecipeRunViewModel: ViewModelBase, IObserver<short>
    {
        public enum RecipeCommand : short { Initiate = 0, Run = 10, Pause = 20, Restart = 30, Stop = 40 };
        public enum RecipeUserState : short  { Uninitialized = -1, Initiated = 0, Run = 10, Pause = 20, Stopped = 40, Ended = 50 };

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

        private class LoadFromRecipeEditSubscriber : IObserver<(string, IList<Recipe>)>
        {
            internal LoadFromRecipeEditSubscriber(RecipeRunViewModel vm)
            {
                recipeRunViewModel = vm;
            }

            void IObserver<(string, IList<Recipe>)>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<(string, IList<Recipe>)>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<(string, IList<Recipe>)>.OnNext((string, IList<Recipe>) value)
            {
               if(0 < value.Item2.Count)
                {
                    recipeRunViewModel.CurrentRecipe = new RecipeContext(value.Item1 != "" ? value.Item1 : DateTime.Now.ToString("yyyyMMddHHmm") + ".csv", value.Item2);
                }
            }

            private RecipeRunViewModel recipeRunViewModel;
        }

        internal class RecipeEndedSubscriber : IObserver<bool>
        {
            internal RecipeEndedSubscriber(RecipeRunViewModel vm)
            {
                recipeRunViewModel = vm;
            }

            void IObserver<bool>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<bool>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<bool>.OnNext(bool value)
            {
                recipeRunViewModel.CurrentRecipeUserState = RecipeUserState.Ended;
            }

            private RecipeRunViewModel recipeRunViewModel;
        }

        bool canRecipeOpenExecute()
        {
            return !canCommandsExecuteOnActive();
        }
        [RelayCommand(CanExecute =nameof(canRecipeOpenExecute))]
        private void RecipeOpen()
        {
            try
            {
                (bool result, string? recipeFilePath, List<Recipe>? recipes) = RecipeService.OpenRecipe(Config);
                if (result == true)
                {
                    if (0 < recipes!.Count)
                    {
                        CurrentRecipe = new RecipeContext(recipeFilePath!, recipes!);
                    }
                    else
                    {
                        MessageBox.Show(recipeFilePath + "은 빈 파일입니다.");
                    }
                }
            }
            catch(Exception exception)
            {
                MessageBox.Show("Recipe를 로드하는데 실패하였습니다. 원인은 다음과 같습니다.\r\n" + exception.Message);
            }
          
        }

        private void startCommand()
        {
            SyncPLCState(Start);
        }

        private void pauseCommand()
        {
            SyncPLCState(RecipeCommand.Pause);
        }

        private bool canStartStopCommadExecute()
        {
            return CurrentRecipeUserState != RecipeUserState.Uninitialized;
        }
        [RelayCommand(CanExecute = nameof(canStartStopCommadExecute))]
        private void RecipeStart()
        {
            startOrStopCommand?.Invoke();
        }

        bool canCommandsExecuteOnActive()
        {
            return RecipeUserState.Run <= CurrentRecipeUserState && CurrentRecipeUserState <= RecipeUserState.Pause;
        }

        bool canSkipCommandExecute()
        {
            return CurrentRecipeUserState == RecipeUserState.Run;
        }
        [RelayCommand(CanExecute = nameof(canSkipCommandExecute))]
        void RecipeSkip()
        {
            PLCService.WriteRCPOperationCommand(60);
        }
     
        bool canRefreshCommandExecute()
        {
            return CurrentRecipeUserState == RecipeUserState.Pause;
        }
        [RelayCommand(CanExecute = nameof(canRefreshCommandExecute))]
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

        [RelayCommand(CanExecute = nameof(canCommandsExecuteOnActive))]
        private void RecipeStop()
        {
            SyncPLCState(RecipeCommand.Stop);  
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

        [RelayCommand]
        private void RecipeClean()
        {
            SyncPLCState(RecipeCommand.Initiate, false);
            CurrentRecipe = EmptyRecipeContext;
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

            PropertyChanging += (object? sender, PropertyChangingEventArgs e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(CurrentRecipe):
                        CurrentRecipe.dispose();
                        break;
                }
            };
            PropertyChanged += (object? sender, PropertyChangedEventArgs e) =>
            {
                var toRecipeLoadedState = () =>
                {
                    StartOrPause = true;
                    CurrentRecipe.toLoadedFromFileState();
                    SyncPLCState(RecipeCommand.Initiate);
                    currentRecipeNo = -1;
                };
                switch (e.PropertyName)
                {
                    case nameof(StartOrPause):
                        switch(StartOrPause)
                        {
                            case false:
                                StartText = "Pause";
                                startOrStopCommand = pauseCommand;
                                break;

                            case true:
                                StartText = "Start";
                                startOrStopCommand = startCommand;
                                break;

                            case null:
                                StartText = "";
                                startOrStopCommand = null; 
                                break;
                        }
                        break;

                    case nameof(CurrentRecipe):
                        if (CurrentRecipe != EmptyRecipeContext)
                        {
                            SyncPLCState(RecipeCommand.Initiate);
                            RecipeService.PLCLoad(CurrentRecipe.Recipes);
                        }
                        else
                        {
                            CurrentRecipeUserState = RecipeUserState.Uninitialized;
                        }
                        break;

                    case nameof(CurrentRecipeUserState):
                        switch(CurrentRecipeUserState)
                        {
                            case RecipeUserState.Uninitialized:
                                StartOrPause = null;
                                Start = RecipeCommand.Run;
                                currentRecipeNo = -1;
                                DashBoardViewModel.resetFlowChart(CurrentRecipe.Recipes);
                                break;

                            case RecipeUserState.Initiated:
                                DashBoardViewModel.resetFlowChart(CurrentRecipe.Recipes);
                                StartOrPause = true;
                                Start = RecipeCommand.Run;
                                break;

                            case RecipeUserState.Run:
                                StartOrPause = false;
                                break;

                            case RecipeUserState.Pause:
                                StartOrPause = true;
                                Start = RecipeCommand.Restart;
                                break;

                            case RecipeUserState.Stopped:
                                toRecipeLoadedState();
                                break;

                            case RecipeUserState.Ended:
                                MessageBox.Show("Recipe가 종료되었습니다." + DateTime.Now.ToString("HH:mm"));
                                toRecipeLoadedState();
                                break;
                        }
                        RecipeOpenCommand.NotifyCanExecuteChanged();
                        RecipeStartCommand.NotifyCanExecuteChanged();
                        RecipeSkipCommand.NotifyCanExecuteChanged();
                        RecipeRefreshCommand.NotifyCanExecuteChanged();
                        RecipeStopCommand.NotifyCanExecuteChanged();
                        recipeRunStatePublisher?.Issue(CurrentRecipeUserState);
                        break;
                }
            };

            ObservableManager<bool>.Subscribe("RecipeEnded", operationStateSubscriber = new RecipeEndedSubscriber(this));
            recipeRunStatePublisher = ObservableManager<RecipeUserState>.Get("RecipeRun.State");
            ObservableManager<(string, IList<Recipe>)>.Subscribe("RecipeEdit.LoadToRecipeRun", loadFromRecipeEditSubscriber = new LoadFromRecipeEditSubscriber(this));
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
            if (currentRecipeNo != value)
            {
                currentRecipeNo = value;
                Recipe? currentRecipe = CurrentRecipe.markCurrent(value);
                if (currentRecipe != null)
                {
                    scrollIntoViewCurrentRecipe(reactorDataGrid, currentRecipe);
                    scrollIntoViewCurrentRecipe(flowDataGrid, currentRecipe);
                    scrollIntoViewCurrentRecipe(valveDataGrid, currentRecipe);
                }
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

        private void SyncPLCState(RecipeCommand command, bool updateState)
        {
            try
            {
                PLCService.WriteRCPOperationCommand((short)command);
                RecipeUserState stateToWait = (command != RecipeCommand.Restart) ? (RecipeUserState)(short)command : RecipeUserState.Run;
                while((RecipeUserState)PLCService.ReadUserState() != stateToWait);
                if (updateState == true)
                {
                    CurrentRecipeUserState = stateToWait;
                }
            }
            catch(Exception)
            {

            }
        }

        private void SyncPLCState(RecipeCommand command)
        {
            SyncPLCState(command, true);
        }

        private static RecipeContext EmptyRecipeContext = new RecipeContext();

        [ObservableProperty]
        private RecipeContext _currentRecipe = EmptyRecipeContext;

        [ObservableProperty]
        private string _startText = "";
        [ObservableProperty]
        private bool? _startOrPause = null;
        private Action? startOrStopCommand = null;

        private static readonly CsvHelper.Configuration.CsvConfiguration Config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true
        };

        public RecipeRunBottomDashBoardViewModel DashBoardViewModel { get; set; }

        private DataGrid? reactorDataGrid;
        private DataGrid? flowDataGrid;
        private DataGrid? valveDataGrid;

        private LogIntervalInRecipeRunListener logIntervalInRecipeRunListener;
        private DispatcherTimer logTimer;

        private short currentRecipeNo = -1;
        private readonly RecipeEndedSubscriber? operationStateSubscriber = null;
        private RecipeCommand Start = RecipeCommand.Run;
        private ObservableManager<RecipeUserState>.DataIssuer recipeRunStatePublisher;
        private LoadFromRecipeEditSubscriber loadFromRecipeEditSubscriber;

        [ObservableProperty]
        private RecipeUserState _currentRecipeUserState = RecipeUserState.Uninitialized;
    }
}
