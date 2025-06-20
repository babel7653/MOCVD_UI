﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SapphireXR_App.Bases;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using SapphireXR_App.ViewModels.BottomDashBoard;

namespace SapphireXR_App.ViewModels
{
    public partial class RecipeRunViewModel: ViewModelBase, IObserver<short>
    {
        public enum RecipeCommand : short { Initiate = 0, Run = 10, Pause = 20, Restart = 30, Stop = 40 };
        public enum RecipeUserState : short  { Uninitialized = -1, Initiated = 0, Run = 10, Pause = 20, Stopped = 40, Ended = 50 };

        public RecipeRunViewModel()
        {
            DashBoardViewModel = new RecipeRunBottomDashBoardViewModel();

            logIntervalInRecipeRunListener = new LogIntervalInRecipeRunListener(this, AppSetting.LogIntervalInRecipeRunInMS);
            ObservableManager<int>.Subscribe("AppSetting.LogIntervalInRecipeRun", logIntervalInRecipeRunListener);
            ObservableManager<short>.Subscribe("RecipeRun.CurrentActiveRecipe", this);
            ObservableManager<bool>.Subscribe("RecipeEnded", recipeEndedSubscriber = new RecipeEndedSubscriber(this));
            recipeRunStatePublisher = ObservableManager<RecipeUserState>.Get("RecipeRun.State");
            ObservableManager<(string, IList<Recipe>)>.Subscribe("RecipeEdit.LoadToRecipeRun", loadFromRecipeEditSubscriber = new LoadFromRecipeEditSubscriber(this));
            recipeEventIssuer = ObservableManager<EventLog>.Get("EventLog");
            ObservableManager<EventLog>.Subscribe("EventLog", eventLogSubscriber = new EventLogSubscriber(this));

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
                switch (e.PropertyName)
                {
                    case nameof(StartOrPause):
                        switch (StartOrPause)
                        {
                            case false:
                                StartText = "Pause";
                                StartPauseIconPath = PauseIconPath;
                                startOrStopCommand = pauseCommand;
                                break;

                            case true:
                                StartText = "Start";
                                StartPauseIconPath = StartIconPath;
                                startOrStopCommand = startCommand;
                                break;

                            case null:
                                StartText = "";
                                StartPauseIconPath = StartIconPath;
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
                            switchState(RecipeUserState.Uninitialized);
                        }
                        break;

                    case nameof(CurrentRecipeUserState):
                 
                        break;
                }
            };

            EventLogs.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs args) => ClearEventLogCommand.NotifyCanExecuteChanged();
            ObservableManager<string>.Get("ViewModelCreated").Issue("RecipeRunViewModel");
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

        public void switchState(RecipeUserState state)
        {
            var toRecipeLoadedState = () =>
            {
                StartOrPause = true;
                CurrentRecipe.toLoadedFromFileState();
                SyncPLCState(RecipeCommand.Initiate);
            };

            CurrentRecipeUserState = state;
            switch (CurrentRecipeUserState)
            {
                case RecipeUserState.Uninitialized:
                    StartOrPause = null;
                    DashBoardViewModel.resetFlowChart(CurrentRecipe.Recipes);
                    break;

                case RecipeUserState.Initiated:
                    DashBoardViewModel.resetFlowChart(CurrentRecipe.Recipes);
                    StartOrPause = true;
                    Start = RecipeCommand.Run;
                    currentRecipeNo = -1;
                    break;

                case RecipeUserState.Run:
                    CurrentRecipe.startLog();
                    if (Start == RecipeCommand.Run)
                    {
                        recipeEventIssuer.Issue(new EventLog() { Date = Util.ToEventLogFormat(DateTime.Now), Message = "레시피가 시작되었습니다", Type = "Recipe Run" });
                    }
                    StartOrPause = false;
                    break;

                case RecipeUserState.Pause:
                    StartOrPause = true;
                    Start = RecipeCommand.Restart;
                    CurrentRecipe.pauseLog();
                    break;

                case RecipeUserState.Stopped:
                    CurrentRecipe.stopLog();
                    recipeEventIssuer.Issue(new EventLog() { Date = Util.ToEventLogFormat(DateTime.Now), Message = "레시피가 중단되었습니다", Type = "Recipe Stop" });
                    toRecipeLoadedState();
                    break;

                case RecipeUserState.Ended:
                    CurrentRecipe.stopLog();
                    recipeEventIssuer.Issue(new EventLog() { Date = Util.ToEventLogFormat(DateTime.Now), Message = "레시피가 종료되었습니다", Type = "Recipe End" });
                    MessageBox.Show("Recipe가 종료되었습니다. 종료시간: " + DateTime.Now.ToString("HH:mm"));
                    toRecipeLoadedState();
                    break;
            }
            RecipeOpenCommand.NotifyCanExecuteChanged();
            RecipeStartCommand.NotifyCanExecuteChanged();
            RecipeSkipCommand.NotifyCanExecuteChanged();
            RecipeRefreshCommand.NotifyCanExecuteChanged();
            RecipeStopCommand.NotifyCanExecuteChanged();
            RecipeCleanCommand.NotifyCanExecuteChanged();
            ChangeLogDirectoryCommand.NotifyCanExecuteChanged();
            recipeRunStatePublisher?.Issue(CurrentRecipeUserState);
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
        bool canCleanCommandExecute()
        {
            return CurrentRecipeUserState == RecipeUserState.Initiated || (RecipeUserState.Stopped <= CurrentRecipeUserState && CurrentRecipeUserState <= RecipeUserState.Ended);
        }
        [RelayCommand(CanExecute = nameof(canCleanCommandExecute))]
        private void RecipeClean()
        {
            SyncPLCState(RecipeCommand.Initiate, false);
            CurrentRecipe = EmptyRecipeContext;
        }

        bool canClearEventLogExecute()
        {
            return 0 < EventLogs.Count;
        }
        [RelayCommand(CanExecute = nameof(canClearEventLogExecute))]
        private void ClearEventLog()
        {
            EventLogs.Clear();
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

                    if (reactorDataGrid?.SelectedItem == currentRecipe)
                    {
                        deselectAllGrid(reactorDataGrid);
                    }
                    if (flowDataGrid?.SelectedItem == currentRecipe)
                    {
                        deselectAllGrid(flowDataGrid);
                    }
                    if (valveDataGrid?.SelectedItem == currentRecipe)
                    {
                        deselectAllGrid(valveDataGrid);
                    }
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

        private static void deselectAllGrid(DataGrid? dataGrid)
        {
            dataGrid?.Dispatcher.InvokeAsync(() =>
            {
                dataGrid.UnselectAll();
            });
        }

        public void resetLogTimer(int intervalInMS)
        {
            //logTimer.Stop();
            //logTimer.Interval = new TimeSpan(intervalInMS * TimeSpan.TicksPerMillisecond);
            //logTimer.Start();
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
                    switchState(stateToWait);
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

        public RelayCommand ChangeLogDirectoryCommand => new RelayCommand(() =>
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            openFolderDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFolderDialog.Multiselect = false;


            if (openFolderDialog.ShowDialog() == true)
            {
                AppSetting.LogFileDirectory = openFolderDialog.FolderName;
            }
        }, () => CurrentRecipeUserState == RecipeUserState.Uninitialized);

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

        private static readonly string StartIconPath = "/Resources/icons/icon=play.png";
        private static readonly string PauseIconPath = "/Resources/icons/icon=stop.png";
        [ObservableProperty]
        private string _startPauseIconPath = StartIconPath;

        private DataGrid? reactorDataGrid;
        private DataGrid? flowDataGrid;
        private DataGrid? valveDataGrid;

        private LogIntervalInRecipeRunListener logIntervalInRecipeRunListener;

        private short currentRecipeNo = -1;
        private readonly RecipeEndedSubscriber? recipeEndedSubscriber = null;
        private RecipeCommand Start = RecipeCommand.Run;
        private ObservableManager<RecipeUserState>.DataIssuer recipeRunStatePublisher;
        private LoadFromRecipeEditSubscriber loadFromRecipeEditSubscriber;

        private ObservableManager<EventLog>.DataIssuer recipeEventIssuer;
        private EventLogSubscriber eventLogSubscriber;

        [ObservableProperty]
        private ObservableCollection<EventLog> _eventLogs = new ObservableCollection<EventLog>();

        [ObservableProperty]
        private RecipeUserState _currentRecipeUserState = RecipeUserState.Uninitialized;
    }
}
