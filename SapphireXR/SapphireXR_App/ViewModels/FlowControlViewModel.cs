using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using SapphireXR_App.WindowServices;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace SapphireXR_App.ViewModels
{
    public partial class FlowControlViewModel : ObservableObject, IObserver<float>
    {
        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private string _message = string.Empty;
        [ObservableProperty]
        private string _targetValue = string.Empty;
        [ObservableProperty]
        private string _rampTime = string.Empty;
        [ObservableProperty]
        private string _deviation = string.Empty;
        [ObservableProperty]
        private string _currentValue = string.Empty;
        [ObservableProperty]
        private string _controlValue = string.Empty;
        [ObservableProperty]
        private int _maxValue;
        [ObservableProperty]
        private SolidColorBrush _fontColor = new SolidColorBrush(Colors.Black);

        public PopupExResult PopupExResult { get; internal set; } = PopupExResult.Close;

        internal class ControlValueSubscriber : IObserver<float>
        {
            public ControlValueSubscriber(FlowControlViewModel viewModel)
            {
                flowControlViewModel = viewModel;
            }
            void IObserver<float>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<float>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<float>.OnNext(float value)
            {

                flowControlViewModel.ControlValue = Util.FloatingPointStrWithMaxDigit(value, AppSetting.FloatingPointMaxNumberDigit);
            }

            private FlowControlViewModel flowControlViewModel;
        }

        internal class CurrentValueSubscriber : IObserver<float>
        {
            public CurrentValueSubscriber(FlowControlViewModel viewModel)
            {
                flowControlViewModel = viewModel;
            }
            void IObserver<float>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<float>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<float>.OnNext(float value)
            {
                flowControlViewModel.CurrentValue = Util.FloatingPointStrWithMaxDigit(value, AppSetting.FloatingPointMaxNumberDigit);
            }

            private FlowControlViewModel flowControlViewModel;
        }

        protected virtual bool canConfirmExecute()
        {
            return TargetValue != string.Empty && RampTime != string.Empty && 0 < short.Parse(RampTime);
        }

        [RelayCommand(CanExecute = "canConfirmExecute")]
        protected virtual void Confirm(Window window)
        {
            PopupExResult = PopupExResult.Confirm;
            if (confirmed(new ControlValues
            {
                targetValue = (string.IsNullOrEmpty(TargetValue) ? null : int.Parse(TargetValue)),
                rampTime = (string.IsNullOrEmpty(RampTime) ? null : short.Parse(RampTime))
            }) == true)
            {
                dispose();
                window.Close();
            }
        }

        [RelayCommand]
        private void Close(Window window)
        {
            PopupExResult = PopupExResult.Close;
            dispose();
            window.Close();
        }

        void dispose()
        {
            controlValueSubscriberDisposable.Dispose();
            currentValueSubscriberDisposable.Dispose();
        }

        void IObserver<float>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<float>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<float>.OnNext(float value)
        {
            throw new NotImplementedException();
        }

        public FlowControlViewModel(string title, string message, string fcID)
        {
            Title = title;
            Message = message;
            controllerID = fcID;
            TargetValue = string.Empty;
            RampTime = string.Empty;
            Deviation = string.Empty;
            CurrentValue = string.Empty;
            ControlValue = string.Empty;
            int? redMaxValue = null;
            if (fcID != "Pressure" || PLCService.ReadPressureControlMode() == 1)
            {
                redMaxValue = SettingViewModel.ReadMaxValue(fcID);
            }
            else
            {
                redMaxValue = 100;
            }
            if (redMaxValue != null && redMaxValue != 0)
            {
                MaxValue = redMaxValue.Value;
            }
            else
            {
                throw new Exception("Faiure happend in reading max value for flow control view window. Logic error in FlowControlViewModel constructor: "
                       + "the value of \"fcID\", the third argument of the constructor \"" + fcID + "\" is not valid flow controller ID:" + redMaxValue == null ? "Null" : redMaxValue.ToString());
            }
            FontColor = OnNormal;
            PropertyChanged += (object? sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "CurrentValue" || e.PropertyName == "ControlValue")
                {
                    if (CurrentValue != string.Empty && ControlValue != string.Empty)
                    {
                        Deviation = Util.FloatingPointStrWithMaxDigit((((float)(Math.Abs(float.Parse(CurrentValue) - float.Parse(ControlValue))) / ((float)MaxValue)) * 100.0f), AppSetting.FloatingPointMaxNumberDigit);
                    }
                }
                switch (e.PropertyName)
                {
                    case nameof(TargetValue):
                    case nameof(RampTime):
                        ConfirmCommand.NotifyCanExecuteChanged();
                        break;
                }
            };
            controlValueSubscriber = new ControlValueSubscriber(this);
            currentValueSubscriber = new CurrentValueSubscriber(this);
            currentValueSubscriberDisposable = ObservableManager<float>.Subscribe("FlowControl." + fcID + ".CurrentValue", currentValueSubscriber);
            controlValueSubscriberDisposable = ObservableManager<float>.Subscribe("FlowControl." + fcID + ".ControlValue", controlValueSubscriber);
        }

        protected virtual bool confirmed(ControlValues controlValues)
        {
            string message = string.Empty;
            if (controlValues.targetValue != null)
            {
                message += "Target Value " + controlValues.targetValue;
            }
            if (controlValues.rampTime != null)
            {
                if (message != string.Empty)
                {
                    message += ", ";
                }
                message += "Ramp Time Value " + controlValues.rampTime;
            }
            if (message != string.Empty)
            {
                message += "으로 설정하시겠습니까?";
            }

            if (message != string.Empty)
            {
                if (FlowControlConfirmEx.Show("변경 확인", message) == DialogResult.Ok)
                {
                    try
                    {
                        if (controlValues.targetValue != null && controlValues.rampTime != null)
                        {
                            PLCService.WriteFlowControllerTargetValue(controllerID, controlValues.targetValue.Value, controlValues.rampTime.Value);
                            //App.Current.MainWindow.Dispatcher.InvokeAsync(() => ToastMessage.Show("PLC로 목표 유량과 램프 시간이 성공적으로 전송되었습니다.", ToastMessage.MessageType.Success));
                            ToastMessage.Show("PLC로 목표 유량과 램프 시간이 성공적으로 전송되었습니다.", ToastMessage.MessageType.Success);
                        }
                    }
                    catch (Exception ex)
                    {
                        //App.Current.MainWindow.Dispatcher.InvokeAsync(() => ToastMessage.Show("PLC로 값을 쓰는데 문제가 발생하였습니다. 자세한 원인은 다음과 같습니다: " + ex.Message, ToastMessage.MessageType.Error));
                        ToastMessage.Show("PLC로 값을 쓰는데 문제가 발생하였습니다. 자세한 원인은 다음과 같습니다: " + ex.Message, ToastMessage.MessageType.Error);
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }

        public struct ControlValues
        {
            public int? targetValue;
            public short? rampTime;
        }

        private static readonly SolidColorBrush OnNormal = new SolidColorBrush(Colors.Red);

        private ControlValueSubscriber controlValueSubscriber;
        private CurrentValueSubscriber currentValueSubscriber;
        private IDisposable currentValueSubscriberDisposable;
        private IDisposable controlValueSubscriberDisposable;
        protected string controllerID;
    }
}
