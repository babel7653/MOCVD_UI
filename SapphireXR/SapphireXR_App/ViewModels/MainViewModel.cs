using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SapphireXR_App.Bases;
using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using SapphireXR_App.WindowServices;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SapphireXR_App.ViewModels
{
    public partial class MainViewModel : ViewModelBase, IObserver<RecipeRunViewModel.RecipeUserState>, IObserver<int>, IObserver<BitArray>, IObserver<PLCService.ControlMode>
    {
        [ObservableProperty]
        private string? navigationSource;

        public MainViewModel()
        {
            Title = "SapphireXR";

             //시작 페이지 설정
            NavigationSource = "Views/RecipeRunPage.xaml";
            //네비게이션 메시지 수신 등록
            WeakReferenceMessenger.Default.Register<NavigationMessage>(this, OnNavigationMessage);

            if (PLCConnectionState.Instance.Online == true)
            {
                changeOperationMode(SelectedTab);
            }

            var updateByPLCState = () =>
            {
                if (RecipeRunInactive == true && NotPriorityState == true)
                {
                    onClosing = confirmClose;
                }
                else
                {
                    onClosing = informNotClosable;
                }
            };

            PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(SelectedTab):
                        if (PLCConnectionState.Instance.Online)
                        {
                            changeOperationMode(SelectedTab);
                        }
                        selectedTabPublisher.Publish(SelectedTab);
                        break;

                    case nameof(RecipeRunInactive):
                        updateByPLCState();
                        break;

                    case nameof(NotPriorityState):
                        updateByPLCState();
                        break;
                }
            };
            onClosing = confirmClose;
            ObservableManager<RecipeRunViewModel.RecipeUserState>.Subscribe("RecipeRun.State", this);
            ObservableManager<int>.Subscribe("SwitchTab", this);
            ObservableManager<BitArray>.Subscribe("LogicalInterlockState", this);
            ObservableManager<PLCService.ControlMode>.Subscribe("ControlModeChanging", this);
            EventLogs.Instance.EventLogList.Add(new EventLog() { Message = "SapphireXR이 시작되었습니다", Name = "Application", Type = EventLog.LogType.Information });
            //EventLogs.Instance.EventLogList.Add(new EventLog() { Message = "M01 Deviation!", Name = "Alarm", Type = EventLog.LogType.Alarm });
            //EventLogs.Instance.EventLogList.Add(new EventLog() { Message = "Time Expire", Name = "Alarm", Type = EventLog.LogType.Alarm });
            //EventLogs.Instance.EventLogList.Add(new EventLog() { Message = "N2 Low", Name = "Warning", Type = EventLog.LogType.Warning });
            //EventLogs.Instance.EventLogList.Add(new EventLog() { Message = "E01 Deviation!", Name = "Alarm", Type = EventLog.LogType.Alarm });
            //EventLogs.Instance.EventLogList.Add(new EventLog() { Message = "NH3 Low", Name = "Warning", Type = EventLog.LogType.Warning });
            //EventLogs.Instance.EventLogList.Add(new EventLog() { Message = "H2 Low", Name = "Warning", Type = EventLog.LogType.Warning });
            //EventLogs.Instance.EventLogList.Add(new EventLog() { Message = "Setting값 저장이 완료되었습니다.", Name = "Application", Type = EventLog.LogType.Information });
            //EventLogs.Instance.EventLogList.Add(new EventLog() { Message = "Setting값 로드가 완료되었습니다.", Name = "Application", Type = EventLog.LogType.Information });

            PLCConnectionState.Instance.PropertyChanged += (sender, args) =>
            {
                switch (PLCConnectionState.Instance.Online)
                {
                    case true:
                        changeOperationMode(SelectedTab);
                        ToastMessage.Show("PLC로 연결되었습니다", ToastMessage.MessageType.Success);
                        break;

                    case false:
                        ToastMessage.Show("PLC로 연결이 끊겼습니다.", ToastMessage.MessageType.Error);
                        break;
                }
            };
            updatePriorityState = () =>
            {
                NotPriorityState = (PLCService.ReadControlMode() != PLCService.ControlMode.Priority);
                if (NotPriorityState == true)
                {
                    Task.Run(() => PLCService.RemovePLCStateUpdateTask(updatePriorityState!));
                }
            };
            try
            {
                MOSourceSetting.Read();
            }
            catch (Exception ex)
            {
                MessageBox.Show(MOSourceSetting.MOSourceFilePath + "을 읽어오면서 오류가 발생하였습니다. MO Source Setting 기능은 사용이 불가합니다. 오류의 원인은 다음과 같습니다:" + ex.Message);
            }
        }

        private void changeOperationMode(int tab)
        {
            try
            {
                switch (tab)
                {
                    case 0:
                        PLCService.WriteControlModeCmd(PLCService.ControlMode.Manual);
                        break;

                    case 1:
                        PLCService.WriteControlModeCmd(PLCService.ControlMode.Recipe);
                        break;
                }
            }
            catch(Exception)
            {
            }
        }

        private void OnNavigationMessage(object recipient, NavigationMessage message)
        {
            NavigationSource = message.Value;
        }

        // 네비게이트 커맨드
        [RelayCommand]
        private void OnNavigate(string pageUri)
        {
            NavigationSource = pageUri;
        }

        public ICommand OnClosingCommand => new RelayCommand<object?>((object? args) =>
        {
            CancelEventArgs? cancelEventArgs = args as CancelEventArgs;
            if (cancelEventArgs != null)
            {
                onClosing(cancelEventArgs);
            }

        });

        private void confirmClose(CancelEventArgs args)
        {
            if (ConfirmMessage.Show("프로그램 종료", "프로그램을 종료하시겠습니까?", WindowStartupLocation.CenterScreen) == DialogResult.Ok)
            {
                closingPublisher.Publish(true);
                AppSetting.Save();
            }
            else
            {
                args.Cancel = true;
            }
        }

        private void informNotClosable(CancelEventArgs args)
        {
            MessageBox.Show("Recipe가 실행 중인 상태에서는 애플리케이션을 종료할 수 없습니다. 현재 실행 중인 Recipe를 중지하거나 Recipe가 실행완료 될 때까지 기다리시기 바랍니다.");
            args.Cancel = true;
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
            RecipeRunInactive = !(RecipeRunViewModel.RecipeUserState.Run <= recipeRunState && recipeRunState <= RecipeRunViewModel.RecipeUserState.Pause);
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
            if (value < 5 && SelectedTab != value)
            {
                SelectedTab = value;
            }
        }

        void IObserver<BitArray>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<BitArray>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<BitArray>.OnNext(BitArray value)
        {
            if (prevAlarmValue != value[0])
            {
                alarmTriggeredPublisher.Publish(value[0]);
                prevAlarmValue = value[0];
            }

            if(value[0] == true && showAlarm == true)
            {
                TriggeredWarningAlarmWindow.Show(PLCService.TriggerType.Alarm, () => showAlarm = true);
                
                showAlarm = false;
            }
            if (value[1] == true && showWarning == true)
            {
                TriggeredWarningAlarmWindow.Show(PLCService.TriggerType.Warning, () => showWarning = true);
                showWarning = false;
            }
        }

        void IObserver<PLCService.ControlMode>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<PLCService.ControlMode>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<PLCService.ControlMode>.OnNext(PLCService.ControlMode value)
        {
            NotPriorityState = (value != PLCService.ControlMode.Priority);
            if (NotPriorityState == false)
            {
                PLCService.AddPLCStateUpdateTask(updatePriorityState);
            }
        }

        private bool showAlarm = true;
        private bool showWarning = true;

        private bool? prevAlarmValue = null;

        [ObservableProperty]
        private int _selectedTab;
        [ObservableProperty]
        private bool recipeRunInactive = true;
        [ObservableProperty]
        private bool notPriorityState = true;

        private Action<CancelEventArgs> onClosing;

        private ObservableManager<int>.Publisher selectedTabPublisher = ObservableManager<int>.Get("MainView.SelectedTabIndex");
        private ObservableManager<bool>.Publisher closingPublisher = ObservableManager<bool>.Get("App.Closing");
        private ObservableManager<bool>.Publisher alarmTriggeredPublisher = ObservableManager<bool>.Get("AlarmTriggered");

        Action updatePriorityState;
    }
}
