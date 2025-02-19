using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.ViewModels.BottomDashBoard;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Common;
using System.Diagnostics.CodeAnalysis;
using SapphireXR_App.Models;

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

        public HomeViewModel()
        {
            DashBoardViewModel = new HomeBottomDashBoardViewModel();
            EnableLeakTestCommand = new RelayCommand<object>((object? menuName) =>
            {
                switch ((string)menuName!)
                {
                    case "Show Leak Test Valve":
                        OnLeakTestVisibility = Visibility.Visible;
                        OffLeakTestVisibility = Visibility.Hidden;
                        LeakTestModeStr = disableLeakTestModeStr;
                        break;

                    case "Hide Leak Test Mode":
                        OnLeakTestVisibility = Visibility.Hidden;
                        OffLeakTestVisibility = Visibility.Visible;
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
        }

        public ICommand EnableLeakTestCommand { get; set; }
        public ICommand ShowValveLabelCommand { get; set; }
        public ICommand ManualBatchCommand => new RelayCommand(() => { ManualBatchEx.Show(); });

        [ObservableProperty]
        private string _leakTestModeStr = disableLeakTestModeStr;
        static private readonly string enableLeakTestModeStr = "Show Leak Test Valve";
        static private readonly string disableLeakTestModeStr = "Hide Leak Test Mode";

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

        public BottomDashBoardViewModel DashBoardViewModel { get; set; }

        private FlowControllerValueSubscriber[] flowControllerValueSubscribers;
    }
}


