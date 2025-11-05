using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using SapphireXR_App.WindowServices;

namespace SapphireXR_App.ViewModels
{
    internal partial class HeaterControlViewModel : FlowControlViewModel
    {
        public enum HeaterControlMode { Auto = 0, Manual = 1 };

        internal HeaterControlViewModel(string title, string message, string fcID) : base(title, message, fcID)
        {
            PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(ControlMode):
                        switch (ControlMode)
                        {
                            case HeaterControlMode.Auto:
                                int? redMaxValue = SettingViewModel.ReadMaxValue(fcID);
                                if (redMaxValue != null)
                                {
                                    MaxValue = (int)redMaxValue;
                                }
                                else
                                {
                                    throw new Exception("Faiure happend in reading max value for flow control view window. Logic error in FlowControlViewModel constructor: "
                                             + "the value of \"fcID\", the third argument of the constructor \"" + fcID + "\" is not valid flow controller ID:" + redMaxValue == null ? "Null" : redMaxValue.ToString());
                                }
                                DeviationEnabled = true;
                                break;

                            case HeaterControlMode.Manual:
                                MaxValue = 100;
                                DeviationEnabled = false;
                                break;
                        }
                        TargetValue = "";
                        RampTimeEnabled = ControlMode == HeaterControlMode.Auto;
                        updateDeviation();
                        break;

                    case nameof(RampTimeEnabled):
                        RampTime = RampTimeEnabled == true ? "" : "0";
                        break;
                }
            };
            ControlMode = PLCService.ReadInputManAuto(7) == true ? HeaterControlMode.Manual : HeaterControlMode.Auto;
            DeviationEnabled = ControlMode == HeaterControlMode.Auto;
        }

        protected override void updateDeviation()
        {
            switch (ControlMode)
            {
                case HeaterControlMode.Auto:
                    base.updateDeviation();
                    break;

                case HeaterControlMode.Manual:
                    Deviation = string.Empty;
                    break;
            }
        }

        protected override bool canConfirmExecute()
        {
            if (ControlMode == HeaterControlMode.Auto)
            {
                return base.canConfirmExecute();
            }
            else
            {
                return TargetValue != string.Empty;
            }
        }

        protected override bool confirmed(ControlValues controlValues)
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
                message = ControlMode + ", " + message;
                if (FlowControlConfirmEx.Show("변경 확인", message) == DialogResult.Ok)
                {
                    try
                    {
                        if (controlValues.targetValue != null && controlValues.rampTime != null)
                        {
                            PLCService.WriteOutputCmd1(PLCService.OutputCmd1Index.TempControllerManAuto, ControlMode == HeaterControlMode.Manual ? true : false);
                            PLCService.WriteFlowControllerTargetValue(controllerID, controlValues.targetValue.Value, ControlMode == HeaterControlMode.Auto ? controlValues.rampTime.Value : (short)0);
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

        [RelayCommand]
        public void ToggleHeaterControlMode()
        {
            ControlMode = (ControlMode == HeaterControlMode.Auto ? HeaterControlMode.Manual : HeaterControlMode.Auto);
        }

        [ObservableProperty]
        private bool rampTimeEnabled = true;
        [ObservableProperty]
        private HeaterControlMode controlMode;
        [ObservableProperty]
        private bool deviationEnabled;
    }
}
