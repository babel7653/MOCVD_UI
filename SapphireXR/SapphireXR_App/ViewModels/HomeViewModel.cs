using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.ViewModels.BottomDashBoard;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.ComponentModel;

namespace SapphireXR_App.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private abstract class FlowControllerValueSubscriber;
        private class FlowControllerValueSubscriber<T> : FlowControllerValueSubscriber, IObserver<T>
        {
            internal FlowControllerValueSubscriber(Action<T> onNextAct, string topicNameStr)
            {
                onNext = onNextAct;
                topicName = topicNameStr;
            }

            void IObserver<T>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<T>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<T>.OnNext(T value)
            {
                onNext(value);
            }

            public string topicName;
            private readonly Action<T> onNext;
        }

        private class DigitalOutput2Subscriber : IObserver<BitArray>
        {
            internal DigitalOutput2Subscriber(HomeViewModel vm)
            {
                homeViewMode = vm;
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
                Util.SetIfChanged(value[(int)PLCService.DigitalOutput2Index.InductionHeaterOn], ref prevInductionHeaterOn, (bool value) => { homeViewMode.InductionHeaterOn = (value == true ? "On" : "Off"); });
                Util.SetIfChanged(value[(int)PLCService.DigitalOutput2Index.InductionHeaterReset], ref prevInductionHeaterReset, (bool value) => { homeViewMode.InductionHeaterReset = (value == true ? "Reset" : "No Reset"); });
                Util.SetIfChanged(value[(int)PLCService.DigitalOutput2Index.VaccumPumpOn], ref prevVaccumPumpOn, (bool value) => { homeViewMode.VaccumPumpOn = (value == true ? "On" : "Off"); });
                Util.SetIfChanged(value[(int)PLCService.DigitalOutput2Index.VaccumPumpReset], ref prevVacuumPumpReset, (bool value) => { homeViewMode.VaccumPumpReset = (value == true ? "Reset" : "No Reset"); });
            }

            private HomeViewModel homeViewMode;
            private bool? prevInductionHeaterOn = null;
            private bool? prevInductionHeaterReset = null;
            private bool? prevVaccumPumpOn = null;
            private bool? prevVacuumPumpReset = null;
        }
        private class DigitalOutput3Subscriber : IObserver<BitArray>
        {
            internal DigitalOutput3Subscriber(HomeViewModel vm)
            {
                homeViewMode = vm;
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
                Util.SetIfChanged(value[(int)PLCService.DigitalOutput3Index.RotationAlaramReset], ref prevRotationAlarmReset, (bool value) => { homeViewMode.RotationReset = (value == true ? "Reset" : "No Reset"); });
            }

            private HomeViewModel homeViewMode;
            private bool? prevRotationAlarmReset;
        }

        private class ThrottleValveControlModeSubscriber : IObserver<short>
        {
            internal ThrottleValveControlModeSubscriber(HomeViewModel vm)
            {
                homeViewModel = vm;
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
                if(prevThrottleValveControlMode == null || prevThrottleValveControlMode != value)
                {
                    switch(value)
                    {
                        case 0:
                          
                            break;

                        case 1:
                            break;

                        case 2:
                            break;

                        case 3:
                            break;

                        case 4:
                            break;
                    }
                    prevThrottleValveControlMode = value;
                }
            }

            private HomeViewModel homeViewModel;
            private short? prevThrottleValveControlMode = null;
        }

        private class InputManAutoSubscriber: IObserver<BitArray>
        {
            internal InputManAutoSubscriber(HomeViewModel vm)
            {
                homeViewModel = vm;
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
                Util.SetIfChanged(value[7], ref prevManAuto, (bool value) => { homeViewModel.InputManualAuto = (value == true ? "Auto" : "Manual"); });

            }

            private HomeViewModel homeViewModel;
            private bool? prevManAuto = null;
        }

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

            flowControllerValueSubscribers = [new FlowControllerValueSubscriber<float>((float value) => { TargetTemp = (int)value; }, "FlowControl.Temperature.TargetValue"),
                new FlowControllerValueSubscriber<int>((int value) => { ControlTemp = (int)value; }, "FlowControl.Temperature.ControlValue"),
                new FlowControllerValueSubscriber<int>((int value) => { CurrentTemp = (int)value; }, "FlowControl.Temperature.CurrentValue"),
                new FlowControllerValueSubscriber<float>((float value) => { PowerRateTemp = (int)value; }, "MonitoringPresentValue.HeaterPowerRate.CurrentValue"),
                new FlowControllerValueSubscriber<float>((float value) => { TargetPress = (int)value; }, "FlowControl.Pressure.TargetValue"),
                new FlowControllerValueSubscriber<int>((int value) => { ControlPress = (int)value; }, "FlowControl.Pressure.ControlValue"),
                new FlowControllerValueSubscriber<int>((int value) => { CurrentPress = (int)value; }, "FlowControl.Pressure.CurrentValue"),
                new FlowControllerValueSubscriber<float>((float value) => { ValvePosition = (int)value; }, "MonitoringPresentValue.ValvePosition.CurrentValue"),
                new FlowControllerValueSubscriber<float>((float value) => { UltimatePressure = (int)value; }, "MonitoringPresentValue.UltimatePressure.CurrentValue"),
                new FlowControllerValueSubscriber<float>((float value) => { TargetRotation = (int)value; }, "FlowControl.Rotation.TargetValue"),
                new FlowControllerValueSubscriber<int>((int value) => { ControlRotation = (int)value; }, "FlowControl.Rotation.ControlValue"),
                new FlowControllerValueSubscriber<int>((int value) => { CurrentRotation = (int)value; }, "FlowControl.Rotation.CurrentValue")];
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
            ObservableManager<BitArray>.Subscribe("DigitalOutput2", digitalOutput2Subscriber = new DigitalOutput2Subscriber(this));
            ObservableManager<BitArray>.Subscribe("DigitalOutput3", digitalOutput3Subscriber = new DigitalOutput3Subscriber(this));
            ObservableManager<BitArray>.Subscribe("InputManAuto", inputManAutoSubscriber = new InputManAutoSubscriber(this));
            ObservableManager<short>.Subscribe("ThrottleValveControlMode", throttleValveControlModeSubscriber = new ThrottleValveControlModeSubscriber(this));
            onPressureControlModeUpdated(PLCService.ReadPressureControlMode());

            ThrottleValveControlModes = ["Control", "Open", "Close", "Hold", "Rest"];
            ushort throttleValveMode = PLCService.ReadThrottleValveMode();
            if(throttleValveMode < ThrottleValveModeCmdToString.Length)
            {
                CurrentThrottleValveControlMode = ThrottleValveModeCmdToString[throttleValveMode];
                prevThrottleValveControlMode = CurrentThrottleValveControlMode;
            }
           
            PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
            {
                if(args.PropertyName == nameof(CurrentThrottleValveControlMode) && CurrentThrottleValveControlMode != null)
                {
                    SynchronizeExpected(ThrottleValveModeStringToCmdOutputMode[CurrentThrottleValveControlMode], PLCService.ReadThrottleValveMode, (ushort throttleValveMode) => prevThrottleValveControlMode = CurrentThrottleValveControlMode,
                            (ushort throttleValveMode) => CurrentThrottleValveControlMode = prevThrottleValveControlMode, 3000,
                            "장비의 Throttle Valve Mode가 " + CurrentThrottleValveControlMode + "값으로 설정되지 않았습니다. 프로그램과 장비 간에 Pressure Control Mode 상태 동기화가 되지 않았습니다.");
                }
            };
        }

        [RelayCommand]
        private void VacuumPumpToggle()
        {
            OutputCmd1OnOffConfirmWindow.Show(VaccumPumpOn, PLCService.OutputCmd2Index.VaccumPumpControl);
        }

        [RelayCommand]
        private void InductionHeaterToggle()
        {
            OutputCmd1OnOffConfirmWindow.Show(InductionHeaterOn, PLCService.OutputCmd2Index.InductionHeaterControl);
        }

        [RelayCommand]
        private void TogglePressureControlMode()
        {
            if(PressureControlMode == PressureControlModePressure)
            {
                PLCService.WriteOutputCmd1(PLCService.OutputCmd2Index.PressureControlMode, true);
                SynchronizeExpected<ushort>(2, PLCService.ReadPressureControlMode, onPressureControlModeUpdated, null, 3000, 
                    "장비의 Pressure Control Mode가 " + PressureControlModePosition + "값으로 설정되지 않았습니다. 프로그램과 장비 간에 Pressure Control Mode 상태 동기화가 되지 않았습니다.");

            }
            else
                if(PressureControlMode == PressureControlModePosition)
                {
                    PLCService.WriteOutputCmd1(PLCService.OutputCmd2Index.PressureControlMode, false);
                    SynchronizeExpected<ushort>(1, PLCService.ReadPressureControlMode, onPressureControlModeUpdated, null,  3000, 
                        "장비의 Pressure Control Mode가 " + PressureControlModePressure + "값으로 설정되지 않았습니다. 프로그램과 장비 간에 Pressure Control Mode 상태 동기화가 되지 않았습니다.");
            }
        }

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

        public ICommand EnableLeakTestCommand { get; set; }
        public ICommand ShowValveLabelCommand { get; set; }
        public ICommand ManualBatchCommand => new RelayCommand(() => { ManualBatchEx.Show(); });

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
        private int _targetTemp;
        [ObservableProperty]
        private int _controlTemp;
        [ObservableProperty]
        private int _currentTemp;
        [ObservableProperty]
        private int _powerRateTemp;
        [ObservableProperty]
        private int _targetPress;
        [ObservableProperty]
        private int _controlPress;
        [ObservableProperty]
        private int _currentPress;
        [ObservableProperty]
        private int _valvePosition;
        [ObservableProperty]
        private int _targetRotation;
        [ObservableProperty]
        private int _controlRotation;
        [ObservableProperty]
        private int _currentRotation;
        [ObservableProperty]
        private int _ultimatePressure;

        [ObservableProperty]
        private string _pressureControlMode = "";
        [ObservableProperty]
        private string _vaccumPumpOn = "";
        [ObservableProperty]
        private string _vaccumPumpReset = "";
        [ObservableProperty]
        private string _inductionHeaterOn = "";
        [ObservableProperty]
        private string _inductionHeaterReset = "";
        [ObservableProperty]
        private string _rotationReset = "";
        [ObservableProperty]
        private string _inputManualAuto = "";
        [ObservableProperty]
        private List<string> _throttleValveControlModes;
        [ObservableProperty]
        private string? _currentThrottleValveControlMode = null;

        private string? prevThrottleValveControlMode = null;

        private static readonly string PressureControlModePressure = "Pressure";
        private static readonly string PressureControlModePosition = "Position";

        private readonly Dictionary<string, ushort> ThrottleValveModeStringToCmdOutputMode = new Dictionary<string, ushort>() { { "Control", 0 }, { "Close", 1 }, { "Open", 2 }, { "Hold", 3 }, { "Reset", 4 } };
        private readonly string[] ThrottleValveModeCmdToString = [ "Control", "Close", "Open", "Hold", "Reset" ];

        public BottomDashBoardViewModel DashBoardViewModel { get; set; }

        private FlowControllerValueSubscriber[] flowControllerValueSubscribers;
        private DigitalOutput2Subscriber digitalOutput2Subscriber;
        private DigitalOutput3Subscriber digitalOutput3Subscriber;
        private InputManAutoSubscriber inputManAutoSubscriber;
        private ThrottleValveControlModeSubscriber throttleValveControlModeSubscriber;
        private ObservableManager<bool>.DataIssuer leakTestModePublisher;
    }
}


