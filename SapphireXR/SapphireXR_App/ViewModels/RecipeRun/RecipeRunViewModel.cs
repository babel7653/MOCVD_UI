using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SapphireXR_App.Bases;
using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using SapphireXR_App.ViewModels.BottomDashBoard;
using SapphireXR_App.WindowServices;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            ObservableManager<BitArray>.Subscribe("LogicalInterlockState", logicalInterlockStateSubscriber = new LogicalInterlockStateSubscriber(this));
            recipeEndedPOnClientSidePublisher = ObservableManager<bool>.Get("RecipeRunViewModel.RecipeEnded");

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
                            try
                            {
                                RecipeService.PLCLoad(CurrentRecipe.Recipes);
                                RecipeService.SetRecipeStepValidator(CurrentRecipe.Recipes, () =>
                                {
                                    RecipeRefreshCommand.NotifyCanExecuteChanged();
                                    RecipeStartCommand.NotifyCanExecuteChanged();
                                });
                                SyncPLCState(RecipeCommand.Initiate);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Recipe를 PLC로 로드하는데 실패했습니다. 원인은 다음과 같습니다: " + ex.Message);
                            }
                        }
                        else
                        {
                            switchState(RecipeUserState.Uninitialized);
                        }
                        break;

                    case nameof(RecipeStartAvailableInterlock):
                        RecipeStartCommand.NotifyCanExecuteChanged();
                        break;
                }
            };

            EventLogs.Instance.EventLogList.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs args) => ClearEventLogCommand.NotifyCanExecuteChanged();
            onPLCConnectionStateChanged(PLCConnectionState.Instance.Online);
            PLCConnectionState.Instance.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(PLCConnectionState.Online))
                {
                    onPLCConnectionStateChanged(PLCConnectionState.Instance.Online);
                }
            };
            ObservableManager<PLCService.ControlMode>.Subscribe("ControlModeChanging", controlModeChangingSubscriber = new ControlModeChangingSubscriber(this));
        }

        bool canRecipeOpenExecute()
        {
            return PLCConnectionState.Instance.Online == true && !canCommandsExecuteOnActive();
        }
        [RelayCommand(CanExecute =nameof(canRecipeOpenExecute))]
        private void RecipeOpen()
        {
            try
            {
                (bool result, string? recipeFilePath, List<Recipe>? recipes, HashSet<string>? fcsMaxValueExceeded) = RecipeService.OpenRecipe(Config, AppSetting.RecipeRunRecipeInitialPath);
                if (result == true)
                {
                    if(recipes!.Count <= 0)
                    {
                        MessageBox.Show("Recipe를 여는데 실패했습니다. 원인은 다음과 같습니다: " + recipeFilePath + "은 빈 파일입니다.");
                        return;
                    }
                    (bool validationResult, string errorMessage) = RecipeValidator.ValidOnLoadedFromDisk(recipes!);
                    if(validationResult == false)
                    {
                        MessageBox.Show("Recipe를 여는데 실패했습니다. 원인은 다음과 같습니다: " + errorMessage);
                        return;
                    }
                    if (AppSetting.MaxNumberOfRecipeSteps < recipes!.Count)
                    {
                        if(ConfirmMessage.Show("", recipeFilePath + "의 Recipe 갯수가 허용가능한 최대갯수인 " + AppSetting.MaxNumberOfRecipeSteps + "을 초과하였습니다." +
                            "확인을 누르면 " + (AppSetting.MaxNumberOfRecipeSteps + 1) + "번 이상의 step들은 삭제된 채 로드됩니다.", WindowStartupLocation.CenterOwner) == DialogResult.Ok)
                        {
                            recipes = recipes!.Take((int)AppSetting.MaxNumberOfRecipeSteps).ToList();
                        }
                        else
                        {
                            return;
                        }
                    }

                    CurrentRecipe = new RecipeContext(recipeFilePath!, recipes!);
                    AppSetting.RecipeRunRecipeInitialPath = Path.GetDirectoryName(recipeFilePath);
                    Util.ShowFlowControllersMaxValueExceededIfExist(fcsMaxValueExceeded);
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
                    CurrentRecipe.onStart();
                    CurrentRecipe.PauseTime = null;
                    if (Start == RecipeCommand.Run)
                    {
                        EventLogs.Instance.EventLogList.Add(new EventLog() { Message = "레시피가 시작되었습니다", Name = "Recipe Run", Type = EventLog.LogType.Information });
                        ToastMessage.Show("Recipe가 시작되었습니다.", ToastMessage.MessageType.Information);
                    }
                    StartOrPause = false;
                    break;

                case RecipeUserState.Pause:
                    StartOrPause = true;
                    CurrentRecipe.onPause();
                    Start = RecipeCommand.Restart;
                    CurrentRecipe.pauseLog();
                    break;

                case RecipeUserState.Stopped:
                    CurrentRecipe.stopLog();
                    EventLogs.Instance.EventLogList.Add(new EventLog() { Message = "레시피가 중단되었습니다", Name = "Recipe Stop", Type = EventLog.LogType.Information });
                    ToastMessage.Show("Recipe가 중단되었습니다.", ToastMessage.MessageType.Information);
                    toRecipeLoadedState();
                    break;

                case RecipeUserState.Ended:
                    CurrentRecipe.stopLog();
                    EventLogs.Instance.EventLogList.Add(new EventLog() { Message = "레시피가 종료되었습니다", Name = "Recipe End", Type = EventLog.LogType.Information });                 
                    ToastMessage.Show("Recipe가 종료되었습니다. 종료시간: " + DateTime.Now.ToString("HH:mm"), ToastMessage.MessageType.Information);
                    toRecipeLoadedState();
                    recipeEndedPOnClientSidePublisher.Publish(true);
                    break;
            }
            RecipeOpenCommand.NotifyCanExecuteChanged();
            RecipeStartCommand.NotifyCanExecuteChanged();
            RecipeSkipCommand.NotifyCanExecuteChanged();
            RecipeRefreshCommand.NotifyCanExecuteChanged();
            RecipeStopCommand.NotifyCanExecuteChanged();
            RecipeCleanCommand.NotifyCanExecuteChanged();
            ChangeLogDirectoryCommand.NotifyCanExecuteChanged();
            recipeRunStatePublisher?.Publish(CurrentRecipeUserState);
            RecipeLoopReadOnly = recipeRunning();
        }

        private bool recipeRunning()
        {
            return RecipeUserState.Run <= CurrentRecipeUserState && CurrentRecipeUserState <= RecipeUserState.Pause;
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
            if (PLCConnectionState.Instance.Online == true && CurrentRecipeUserState != RecipeUserState.Uninitialized && RecipeStartAvailableInterlock == true)
            {
                if(0 < CurrentRecipe.Recipes.Count)
                {
                    return RecipeValidator.Valid(CurrentRecipe.Recipes);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        [RelayCommand(CanExecute = nameof(canStartStopCommadExecute))]
        private void RecipeStart()
        {
            startOrStopCommand?.Invoke();
        }

        bool canCommandsExecuteOnActive()
        {
            return PLCConnectionState.Instance.Online == true && recipeRunning();
        }

        bool canSkipCommandExecute()
        {
            return PLCConnectionState.Instance.Online == true && CurrentRecipeUserState == RecipeUserState.Run && SkipEnable == true;
        }
        [RelayCommand(CanExecute = nameof(canSkipCommandExecute))]
        void RecipeSkip()
        {
            PLCService.WriteRCPOperationCommand(60);
            SkipEnable = false;
            Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(task => SkipEnable = true, TaskScheduler.FromCurrentSynchronizationContext());
        }
     
        bool canRefreshCommandExecute()
        {
            if(PLCConnectionState.Instance.Online == true && CurrentRecipeUserState != RecipeUserState.Run)
            {
                if(0 < CurrentRecipe.Recipes.Count)
                {
                    return RecipeValidator.Valid(CurrentRecipe.Recipes);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
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
            return PLCConnectionState.Instance.Online == true && CurrentRecipeUserState == RecipeUserState.Initiated || (RecipeUserState.Stopped <= CurrentRecipeUserState && CurrentRecipeUserState <= RecipeUserState.Ended);
        }
        [RelayCommand(CanExecute = nameof(canCleanCommandExecute))]
        private void RecipeClean()
        {
            SyncPLCState(RecipeCommand.Initiate, false);
            CurrentRecipe = EmptyRecipeContext;
        }

        bool canClearEventLogExecute()
        {
            return 0 < EventLogs.Instance.EventLogList.Count();
        }
        [RelayCommand(CanExecute = nameof(canClearEventLogExecute))]
        private void ClearEventLog()
        {
            EventLogs.Instance.EventLogList.Clear();
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

        private void onPLCConnectionStateChanged(bool connected)
        {
            RecipeOpenCommand.NotifyCanExecuteChanged();
            RecipeStartCommand.NotifyCanExecuteChanged();
            RecipeSkipCommand.NotifyCanExecuteChanged();
            RecipeRefreshCommand.NotifyCanExecuteChanged();
            RecipeStopCommand.NotifyCanExecuteChanged();
            RecipeCleanCommand.NotifyCanExecuteChanged();
            if(connected == true)
            {
                RecipeStartAvailableInterlock = PLCService.ReadRecipeStartAvailable();
            }
        }

        private void SyncPLCState(RecipeCommand command, bool updateState)
        {
            try
            {
                PLCService.WriteRCPOperationCommand((short)command);
                RecipeUserState stateToWait = (command != RecipeCommand.Restart) ? (RecipeUserState)(short)command : RecipeUserState.Run;
                
                DateTime startTime = DateTime.Now;
                while ((RecipeUserState)PLCService.ReadUserState() != stateToWait)
                {
                    if(10 < (DateTime.Now - startTime).Seconds)
                    {
                        throw new TimeoutException("레시피 상태를 PLC로부터 읽기와 관련된 설정 Timeout값 10초가 초과되었습니다. PLC와의 연결을 확인해주세요.");
                    }
                }

                if (updateState == true)
                {
                    switchState(stateToWait);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("레시피 상태를 PLC로부터 읽어오는 중에 오류가 발생하였습니다.\r\n오류 내용: " + e.Message + "\r\n" + 
                    "Sapphire 애플리케이션과 PLC를 전부 재시작하는 것을 권장드립니다.", "레시피 상태 동기화 오류");
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
        public ICommand OnDoubleClickedCommand => new RelayCommand(() => { EventLogWindow.Show(); });

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
        private ObservableManager<RecipeUserState>.Publisher recipeRunStatePublisher;
        private LoadFromRecipeEditSubscriber loadFromRecipeEditSubscriber;
        private LogicalInterlockStateSubscriber logicalInterlockStateSubscriber;
        private ObservableManager<bool>.Publisher recipeEndedPOnClientSidePublisher;
        private ControlModeChangingSubscriber controlModeChangingSubscriber;

        private bool skipEnable = true;
        private bool SkipEnable
        {
            get => skipEnable;
            set
            {
                skipEnable = value;
                RecipeSkipCommand.NotifyCanExecuteChanged();
            }
        }

        [ObservableProperty]
        private bool recipeStartAvailableInterlock = false;

        [ObservableProperty]
        private RecipeUserState _currentRecipeUserState = RecipeUserState.Uninitialized;

        [ObservableProperty]
        private bool recipeLoopReadOnly = true;
    }
}
