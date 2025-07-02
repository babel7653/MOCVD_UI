using SapphireXR_App.Enums;
using SapphireXR_App.Common;
using SapphireXR_App.WindowServices;
using static SapphireXR_App.ViewModels.FlowControlViewModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using SapphireXR_App.Models;

namespace SapphireXR_App.ViewModels.FlowController
{
    public partial class HomeFlowControllerViewModel : FlowControllerViewModelBase
    {
        protected class ControlTargetValueSubscriber : IObserver<(float, float)>
        {
            public ControlTargetValueSubscriber(HomeFlowControllerViewModel viewModel)
            {
                flowControllerViewModel = viewModel;

                int? redMaxValue = SettingViewModel.ReadMaxValue(viewModel.ControllerID);
                if (redMaxValue != null)
                {
                    maxValue = (float)redMaxValue;
                }
                else
                {
                    throw new Exception("Faiure happend in reading max value for flow controller in home view. Logic error in ControlTargetValueSubscriber contructor in HomeFlowControllerViewModel: "
                        + "the value of \"viewModel.ControllerID\", the constructor argument \"" + viewModel.ControllerID + "\" is not valid flow controller ID");
                }
            }
            void IObserver<(float, float)>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<(float, float)>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<(float, float)>.OnNext((float, float) values)
            {
                if (values.Item1 / maxValue < AppSetting.UnderFlowControlFallbackRate)
                {
                    values.Item1 = 0.0f;
                }
                string controlValue = Util.FloatingPointStrWithMaxDigit(values.Item1, AppSetting.FloatingPointMaxNumberDigit);
                if (controlValue != flowControllerViewModel.ControlValue)
                {
                    flowControllerViewModel.ControlValue = controlValue;
                }
                string targetValue = Util.FloatingPointStrWithMaxDigit(values.Item2, AppSetting.FloatingPointMaxNumberDigit);
                if (targetValue != flowControllerViewModel.TargetValue)
                {
                    flowControllerViewModel.TargetValue = targetValue;
                }
            }
            
            private HomeFlowControllerViewModel flowControllerViewModel;
            private float maxValue;
        }

        public bool OnFlowControllerConfirmed(PopupExResult result, ControlValues controlValues)
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
                if (FlowControlConfirmEx.Show("변경 확인", message) == ValveOperationExResult.Ok)
                {
                    try
                    {
                        if (controlValues.targetValue != null)
                        {
                            PLCService.WriteTargetValue(ControllerID, (int)controlValues.targetValue);
                        }
                        if (controlValues.rampTime != null)
                        {
                            PLCService.WriteRampTime(ControllerID, (short)controlValues.rampTime);
                        }
                        ToastMessage.Show(ControllerID + " Target Value, Ramp Time 설정 완료", ToastMessage.MessageType.Sucess);
                    }
                    catch (Exception ex)
                    {
                        ToastMessage.Show("PLC로 값을 쓰는데 문제가 발생하였습니다. 자세한 원인은 다음과 같습니다: " + ex.Message, ToastMessage.MessageType.Error);
                        return false;
                    }

                    return true;
                }
            }

            return false;
        }
        public void OnFlowControllerCanceled(PopupExResult result)
        {

        }

        protected override void onLoaded(string type, string controllerID)
        {
            base.onLoaded(type, controllerID);
            ObservableManager<(float, float)>.Subscribe("FlowControl." + ControllerID + ".ControlTargetValue.CurrentPLCState", controlTargetValueSubscriber = new ControlTargetValueSubscriber(this));
            selectedThis = ObservableManager<string>.Get("FlowControl.Selected.CurrentPLCState.Home");
        }

        protected override void onClicked(object[]? args)
        {
            switch(PLCService.Connected)
            {
                case PLCConnection.Connected:
                    selectedThis?.Issue(ControllerID);
                    break;

                case PLCConnection.Disconnected:
                    MessageBox.Show("현재 PLC에 연결되어 있지 않습니다.");
                    break;
            }
        }


        private 

        protected ControlTargetValueSubscriber? controlTargetValueSubscriber;
        private ObservableManager<string>.Publisher? selectedThis;

        [ObservableProperty]
        private string _currentValue = "";
        [ObservableProperty]
        private string _targetValue = "";
    }
}
