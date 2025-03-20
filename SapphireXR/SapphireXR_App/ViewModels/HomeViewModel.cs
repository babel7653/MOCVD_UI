using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.ViewModels.BottomDashBoard;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using System.Collections;
using System.Numerics;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;

namespace SapphireXR_App.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        public HomeViewModel()
        {
            DashBoardViewModel = new HomeBottomDashBoardViewModel();
            leakTestModePublisher = ObservableManager<bool>.Get("Leak Test Mode");
            EnableLeakTestCommand = new RelayCommand<object>((object? menuName) =>
            {
                switch ((string)menuName!)
                {
                    case "Show Leak Test Valve":
                        OnLeakTestVisibility = Visibility.Visible;
                        OffLeakTestVisibility = Visibility.Hidden;
                        leakTestModePublisher.Issue(true);
                        LeakTestModeStr = disableLeakTestModeStr;
                        break;

                    case "Hide Leak Test Valve":
                        OnLeakTestVisibility = Visibility.Hidden;
                        OffLeakTestVisibility = Visibility.Visible;
                        leakTestModePublisher.Issue(false);
                        LeakTestModeStr = enableLeakTestModeStr;
                        break;
                }
            });

            ShowValveLabelCommand = new RelayCommand<object>((object? menuName) =>
            {
                switch ((string)menuName!)
                {
                    case "Show Label":
                        ValveLabelVisibility = Visibility.Visible;
                        ShowValveLabelStr = hideValveLabelStr;
                        break;
                    case "Hide Label":
                        ValveLabelVisibility = Visibility.Hidden;
                        ShowValveLabelStr = showValveLabelStr;
                        break;
                }
            });

            flowControllerValueSubscribers = [new FlowControllerValueSubscriber<float>((float value) => { TargetTemp = Util.FloatingPointStrWithMaxDigit(value, AppSetting.FloatingPointMaxNumberDigit); }, 
                "FlowControl.Temperature.TargetValue"),
                new FlowControllerValueSubscriber<float>((float value) => { ControlTemp = Util.FloatingPointStrWithMaxDigit(value, AppSetting.FloatingPointMaxNumberDigit); }, 
                "FlowControl.Temperature.ControlValue"),
                new FlowControllerValueSubscriber<float>((float value) => { CurrentTemp = Util.FloatingPointStrWithMaxDigit(value, AppSetting.FloatingPointMaxNumberDigit); }, 
                "FlowControl.Temperature.CurrentValue"),
                new FlowControllerValueSubscriber<float>((float value) => { PowerRateTemp = Util.FloatingPointStrWithMaxDigit(value, AppSetting.FloatingPointMaxNumberDigit); }, 
                "MonitoringPresentValue.HeaterPowerRate.CurrentValue"),
                new FlowControllerValueSubscriber<float>((float value) => { TargetPress =  Util.FloatingPointStrWithMaxDigit(value, AppSetting.FloatingPointMaxNumberDigit); }, 
                "FlowControl.Pressure.TargetValue"),
                new FlowControllerValueSubscriber<float>((float value) => { ControlPress =  Util.FloatingPointStrWithMaxDigit(value, AppSetting.FloatingPointMaxNumberDigit); }, 
                "FlowControl.Pressure.ControlValue"),
                new FlowControllerValueSubscriber<float>((float value) => { CurrentPress =  Util.FloatingPointStrWithMaxDigit(value, AppSetting.FloatingPointMaxNumberDigit); }, 
                "FlowControl.Pressure.CurrentValue"),
                new FlowControllerValueSubscriber<float>((float value) => { ValvePosition = Util.FloatingPointStrWithMaxDigit(value, AppSetting.FloatingPointMaxNumberDigit); }, 
                "MonitoringPresentValue.ValvePosition.CurrentValue"),
                new FlowControllerValueSubscriber<float>((float value) => { UltimatePressure =Util.FloatingPointStrWithMaxDigit(value, AppSetting.FloatingPointMaxNumberDigit); }, 
                "MonitoringPresentValue.UltimatePressure.CurrentValue"),
                new FlowControllerValueSubscriber<float>((float value) => { TargetRotation = Util.FloatingPointStrWithMaxDigit(value, AppSetting.FloatingPointMaxNumberDigit); }, 
                "FlowControl.Rotation.TargetValue"),
                new FlowControllerValueSubscriber<float>((float value) => { ControlRotation =Util.FloatingPointStrWithMaxDigit(value, AppSetting.FloatingPointMaxNumberDigit); }, 
                "FlowControl.Rotation.ControlValue"),
                new FlowControllerValueSubscriber<float>((float value) => { CurrentRotation = Util.FloatingPointStrWithMaxDigit(value, AppSetting.FloatingPointMaxNumberDigit); }, 
                "FlowControl.Rotation.CurrentValue")];
            foreach(FlowControllerValueSubscriber subscriber in flowControllerValueSubscribers)
            {
                if(subscriber is FlowControllerValueSubscriber<int>)
                {
                    ObservableManager<int>.Subscribe(((FlowControllerValueSubscriber<int>)subscriber).topicName, (FlowControllerValueSubscriber<int>)subscriber);
                }
                else
                {
                    ObservableManager<float>.Subscribe(((FlowControllerValueSubscriber<float>)subscriber).topicName, (FlowControllerValueSubscriber<float>)subscriber);
                }
            }
       
            ObservableManager<BitArray>.Subscribe("DigitalOutput3", digitalOutput3Subscriber = new DigitalOutput3Subscriber(this));
            ObservableManager<short>.Subscribe("ThrottleValveStatus", throttleValveStatusSubscriber = new ThrottleValveStatusSubscriber(this));
            onPressureControlModeUpdated(PLCService.ReadPressureControlMode());

            ThrottleValveControlModes = ["Control", "Open", "Close", "Hold", "Reset"];
            ushort throttleValveMode = PLCService.ReadThrottleValveMode();
            if(throttleValveMode < ThrottleValveModeCmdToString.Length)
            {
                CurrentThrottleValveControlMode = ThrottleValveModeCmdToString[throttleValveMode];
                prevThrottleValveControlMode = CurrentThrottleValveControlMode;
            }

            BitArray outputCmd1 = PLCService.ReadOutputCmd1();
            InductionHeaterOn = (outputCmd1[(int)PLCService.OutputCmd1Index.InductionHeaterControl] == true) ? "On" : "Off";
            VaccumPumpOn = (outputCmd1[(int)PLCService.OutputCmd1Index.VaccumPumpControl] == true) ? "On" : "Off";
            InputManualAuto = PLCService.ReadInputManAuto(7) == false ? "Auto" : "Manual";

            PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
            {
                if(args.PropertyName == nameof(ThrottleValveStatus))
                {
                    VacuumPumpResetCommand.NotifyCanExecuteChanged();
                }
            };

            EventLogs.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs args) => ClearEventLogsCommand.NotifyCanExecuteChanged();
            EventLogs.Add(new() { Date = Util.ToEventLogFormat(App.AppStartTime), Message = "SapphireXR 시작", Type = "Application" });
            ObservableManager<EventLog>.Subscribe("EventLog", eventLogSubscriber = new EventLogSubscriber(this));
        }


        [RelayCommand]
        private void VacuumPumpToggle()
        {
            if(OutputCmd1ToggleConfirmService.OnOff(VaccumPumpOn, PLCService.OutputCmd1Index.VaccumPumpControl, "Vaccum Pump On/Off") == true)
            {
                VaccumPumpOn = (VaccumPumpOn == "On" ? "Off" : "On");
            }
        }

        bool CanVacuumPumpResetExecute()
        {
            return ThrottleValveStatus == "Valve Fault";
        }

        [RelayCommand(CanExecute = "CanVacuumPumpResetExecute")]
        private void VacuumPumpReset()
        {
            if (ValveOperationEx.Show("Vaccum Pump Reset", "Reset 하시겠습니까?") == Enums.ValveOperationExResult.Ok)
            {
                PLCService.WriteOutputCmd1(PLCService.OutputCmd1Index.VaccumPumpReset, true);
            }
        }

        [RelayCommand]
        private void ToggleHeaterControlMode()
        {
            string nextState = InputManualAuto == "Auto" ? "Manual" : "Auto";
            if (OutputCmd1ToggleConfirmService.Toggle(PLCService.OutputCmd1Index.TempControllerManAuto, "Induction Power Supply Manual/Auto",nextState + " 상태로 바꾸시겠습니까?", InputManualAuto, 
                "Manual", "Auto") == true)
            {
                SynchronizeExpected(InputManualAuto == "Auto" ? 1 : 0, () => (PLCService.ReadInputManAuto(7) == true ? 1 : 0), (int manualAuto) => InputManualAuto = (manualAuto == 0 ? "Auto" : "Manual"),
                    null, 3000, "장비의 Input Heater Control Mode가 " + nextState + "로 설정되지 않았습니다. 프로그램과 장비 간에 Heater Control Mode 상태 동기화가 되지 않았습니다.");
            }
        }

        [RelayCommand]
        private void InductionHeaterToggle()
        {
            if(OutputCmd1ToggleConfirmService.OnOff(InductionHeaterOn, PLCService.OutputCmd1Index.InductionHeaterControl, "Induction Power Supply On/Off") == true)
            {
                InductionHeaterOn = (InductionHeaterOn == "On" ? "Off" : "On");
            }
        }

        [RelayCommand]
        private void InductionHeaterReset()
        {
            if (ValveOperationEx.Show("Vaccum Pump Reset", "Reset 하시겠습니까?") == Enums.ValveOperationExResult.Ok)
            {
                PLCService.WriteOutputCmd1(PLCService.OutputCmd1Index.InductionHeaterReset, true);
                //int timeout = 10000;
                //SynchronizeExpected(0, () => PLCService.ReadDigitalOutputIO2(1) == true ? 1 : 0, null, null, timeout, "Induction Heater Reset 명령이 실패하였거나 본 프로그램의 timout 대기 시간 " +
                //    timeout + "(MS)을 초과하셨습니다");
            }
        }

        [RelayCommand(CanExecute = "CanClearEventLogsExecute")]
        private void ClearEventLogs()
        {
            EventLogs.Clear();
        }

        private bool CanClearEventLogsExecute()
        {
            return 0 < EventLogs.Count;
        }

        [RelayCommand]
        private void TogglePressureControlMode()
        {
            if (ValveOperationEx.Show("Pressure Control Mode 변경", (PressureControlMode == PressureControlModePressure ? PressureControlModePosition : PressureControlModePressure) + "로 변경하시겠습니까?") == Enums.ValveOperationExResult.Ok)
            {
                if (PressureControlMode == PressureControlModePressure)
                {
                    PLCService.WriteOutputCmd1(PLCService.OutputCmd1Index.PressureControlMode, true);
                    SynchronizeExpected<ushort>(2, PLCService.ReadPressureControlMode, onPressureControlModeUpdated, null, 3000,
                        "장비의 Pressure Control Mode가 " + PressureControlModePosition + "값으로 설정되지 않았습니다. 프로그램과 장비 간에 Pressure Control Mode 상태 동기화가 되지 않았습니다.");

                }
                else
                    if (PressureControlMode == PressureControlModePosition)
                {
                    PLCService.WriteOutputCmd1(PLCService.OutputCmd1Index.PressureControlMode, false);
                    SynchronizeExpected<ushort>(1, PLCService.ReadPressureControlMode, onPressureControlModeUpdated, null, 3000,
                        "장비의 Pressure Control Mode가 " + PressureControlModePressure + "값으로 설정되지 않았습니다. 프로그램과 장비 간에 Pressure Control Mode 상태 동기화가 되지 않았습니다.");
                }
            }
        }

        public ICommand OnThrottleValveModeChangedCommand => new RelayCommand<object?>((object? args) =>
        {
            SelectionChangedEventArgs? selectionChangedEventArgs = args as SelectionChangedEventArgs;
            if (selectionChangedEventArgs != null)
            {
                ComboBox? comboBox = selectionChangedEventArgs.Source as ComboBox;
                if (comboBox != null)
                {
                    string? selectedMode = comboBox.SelectedItem as string;
                    if (selectedMode != null)
                    {
                        ushort cmdOutputMode = ThrottleValveModeStringToCmdOutputMode[selectedMode];
                        PLCService.WriteThrottleValveMode((short)cmdOutputMode);
                        SynchronizeExpected(cmdOutputMode, PLCService.ReadThrottleValveMode, (ushort throttleValveMode) => prevThrottleValveControlMode = CurrentThrottleValveControlMode,
                                (ushort throttleValveMode) => CurrentThrottleValveControlMode = prevThrottleValveControlMode, 3000,
                                "장비의 Throttle Valve Mode가 " + CurrentThrottleValveControlMode + "값으로 설정되지 않았습니다. 프로그램과 장비 간에 Pressure Control Mode 상태 동기화가 되지 않았습니다.");
                    }
                }
            }
        });


        private static void SynchronizeExpected<T>(T expected, Func<T> checkFunc, Action<T>? onSync, Action<T>? onFailed, long timeOutMS, string messageOnTimeout) where T : INumber<T>
        {
            if(Util.SynchronizeExpected(expected, checkFunc, timeOutMS) == true)
            {
                onSync?.Invoke(expected);
            }
            else
            {
                onFailed?.Invoke(expected);
                MessageBox.Show(messageOnTimeout);
            }
        }

        private void onPressureControlModeUpdated(ushort mode)
        {
            switch(mode)
            {
                case 1:
                    PressureControlMode = PressureControlModePressure;
                    break;
                case 2:
                    PressureControlMode = PressureControlModePosition;
                    break;
            }
        }

        private void loadBatchOnRecipeEnd()
        {
            if(batchOnRecipeEnd != null)
            {
                Util.LoadBatchToPLC(batchOnRecipeEnd);
            }
        }

        private void loadBatchOnAlaramState()
        {
            if(batchOnAlarmState != null)
            {
                Util.LoadBatchToPLC(batchOnAlarmState);
            }
        }

        public ICommand EnableLeakTestCommand { get; set; }
        public ICommand ShowValveLabelCommand { get; set; }
        public ICommand ManualBatchCommand => new RelayCommand(() => { 
            (batchOnAlarmState, batchOnRecipeEnd) = ManualBatchEx.Show();
            if (batchOnAlarmState != null)
            {
                AppSetting.BatchOnAlarmState = batchOnAlarmState.Name;
            }
            if(batchOnRecipeEnd != null)
            {
                AppSetting.BatchOnRecipeEnd = batchOnRecipeEnd.Name;
            }

        });

        [ObservableProperty]
        private string _leakTestModeStr = disableLeakTestModeStr;
        static private readonly string enableLeakTestModeStr = "Show Leak Test Valve";
        static private readonly string disableLeakTestModeStr = "Hide Leak Test Valve";

        [ObservableProperty]
        private string _showValveLabelStr = hideValveLabelStr;
        static private readonly string showValveLabelStr = "Show Label";
        static private readonly string hideValveLabelStr = "Hide Label";

        [ObservableProperty]
        private Visibility _onLeakTestVisibility;
        [ObservableProperty]
        private Visibility _offLeakTestVisibility;
        [ObservableProperty]
        private Visibility _valveLabelVisibility;

        [ObservableProperty]
        private string _targetTemp = "";
        [ObservableProperty]
        private string _controlTemp = "";
        [ObservableProperty]
        private string _currentTemp = "";
        [ObservableProperty]
        private string _powerRateTemp = "";
        [ObservableProperty]
        private string _targetPress = "";
        [ObservableProperty]
        private string _controlPress = "";
        [ObservableProperty]
        private string _currentPress = "";
        [ObservableProperty]
        private string _valvePosition = "";
        [ObservableProperty]
        private string _targetRotation = "";
        [ObservableProperty]
        private string _controlRotation = "";
        [ObservableProperty]
        private string _currentRotation = "";
        [ObservableProperty]
        private string _ultimatePressure = "";

        [ObservableProperty]
        private string _pressureControlMode = "";
        [ObservableProperty]
        private string _vaccumPumpOn = "";
        [ObservableProperty]
        private string _vaccumPumpReset = "";
        [ObservableProperty]
        private string _inductionHeaterOn = "";
        [ObservableProperty]
        private string _rotationReset = "";
        [ObservableProperty]
        private string _inputManualAuto = "";
        [ObservableProperty]
        private List<string> _throttleValveControlModes;
        [ObservableProperty]
        private string? _currentThrottleValveControlMode = null;
        [ObservableProperty]
        private string _throttleValveStatus = "";

        [ObservableProperty]
        ObservableCollection<EventLog> _eventLogs = new ObservableCollection<EventLog>();

        private string? prevThrottleValveControlMode = null;

        private static readonly string PressureControlModePressure = "Pressure";
        private static readonly string PressureControlModePosition = "Position";

        private ManualBatchViewModel.Batch? batchOnAlarmState;
        private ManualBatchViewModel.Batch? batchOnRecipeEnd;

        private static readonly Dictionary<string, ushort> ThrottleValveModeStringToCmdOutputMode = new Dictionary<string, ushort>() { { "Control", 0 }, { "Close", 1 }, { "Open", 2 }, { "Hold", 3 }, { "Reset", 4 } };
        private static readonly string[] ThrottleValveModeCmdToString = [ "Control", "Close", "Open", "Hold", "Reset"];
        private static readonly string[] ThrottleValveStatusToString = ["Normal", "Position Control", "Valve Open", "Not Initialized", "Valve Closed", "Valve Fault", "Valve Initializing", "Pressure Control", "Valve Hold"];

        public BottomDashBoardViewModel DashBoardViewModel { get; set; }

        private FlowControllerValueSubscriber[] flowControllerValueSubscribers;
        private DigitalOutput3Subscriber digitalOutput3Subscriber;
        private ThrottleValveStatusSubscriber throttleValveStatusSubscriber;
        private ObservableManager<bool>.DataIssuer leakTestModePublisher;
        private EventLogSubscriber eventLogSubscriber;
    }
}


