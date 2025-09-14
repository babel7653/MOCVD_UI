using SapphireXR_App.Enums;
using SapphireXR_App.Common;
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
                if(maxValue == 0)
                {
                    throw new InvalidOperationException("Max value for flow controller cannot be zero. Logic error in ControlTargetValueSubscriber.OnNext method in HomeFlowControllerViewModel: "
                        + "the value of \"viewModel.ControllerID\", the constructor argument \"" + flowControllerViewModel.ControllerID + "\" is not valid flow controller ID");
                }

                if ((values.Item1 / maxValue) < AppSetting.UnderFlowControlFallbackRate)
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
                    selectedThis?.Publish(ControllerID);
                    break;

                case PLCConnection.Disconnected:
                    MessageBox.Show("현재 PLC에 연결되어 있지 않습니다.");
                    break;
            }
        }

        protected ControlTargetValueSubscriber? controlTargetValueSubscriber;
        private ObservableManager<string>.Publisher? selectedThis;

        [ObservableProperty]
        private string _currentValue = "";
        [ObservableProperty]
        private string _targetValue = "";
    }
}
