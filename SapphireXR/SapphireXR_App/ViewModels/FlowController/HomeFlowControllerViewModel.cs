using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using SapphireXR_App.Enums;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using static SapphireXR_App.ViewModels.FlowControlViewModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;

namespace SapphireXR_App.ViewModels.FlowController
{
    public partial class HomeFlowControllerViewModel : FlowControllerViewModelBase
    {
        protected class ControlTargetValueSubscriber : IObserver<(float, float)>
        {
            public ControlTargetValueSubscriber(HomeFlowControllerViewModel viewModel)
            {
                flowControllerViewModel = viewModel;
                maxValue = PLCService.ReadMaxValue(viewModel.ControllerID);
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
                string controlValue = values.Item1.ToString("N", new NumberFormatInfo() { NumberDecimalDigits = Util.NumberDecimalDigits(values.Item1, AppSetting.MaxNumberDigit) });
                if(controlValue != flowControllerViewModel.ControlValue)
                {
                    flowControllerViewModel.ControlValue = controlValue;
                }
                string targetValue = values.Item2.ToString("N", new NumberFormatInfo() { NumberDecimalDigits = Util.NumberDecimalDigits(values.Item2, AppSetting.MaxNumberDigit) });
                if (targetValue != flowControllerViewModel.TargetValue)
                {
                    flowControllerViewModel.TargetValue = targetValue;
                }
            }
            
            private HomeFlowControllerViewModel flowControllerViewModel;
            private float maxValue;
        }

        public HomeFlowControllerViewModel()
        {
            OnFlowControllerConfirmedCommand = new RelayCommand<object?>((parameter) =>
            {
                object[] parameters = (object[])parameter!;
                PopupExResult result = (PopupExResult)parameters[0];
                ControlValues controlValues = (ControlValues)parameters[1];

                if (controlValues.targetValue != null)
                {
                    PLCService.WriteTargetValue(ControllerID, (int)controlValues.targetValue);
                }
                if (controlValues.rampTime != null)
                {
                    PLCService.WriteRampTime(ControllerID, (short)controlValues.rampTime);
                }
            });
        }

        protected override void onLoaded(string type, string controllerID)
        {
            base.onLoaded(type, controllerID);
            ObservableManager<(float, float)>.Subscribe("FlowControl." + ControllerID + ".ControlTargetValue.CurrentPLCState", controlTargetValueSubscriber = new ControlTargetValueSubscriber(this));
            selectedThis = ObservableManager<string>.Get("FlowControl.Selected.CurrentPLCState");
        }

        protected override void onClicked(object[]? args)
        {
            selectedThis?.Issue(ControllerID);
        }
      
        public ICommand OnFlowControllerConfirmedCommand;
        public ICommand OnFlowControllerCanceledCommand = new RelayCommand<PopupExResult>((result) => { });

        protected ControlTargetValueSubscriber? controlTargetValueSubscriber;
        private ObservableManager<string>.DataIssuer? selectedThis;

        [ObservableProperty]
        private string _currentValue = "";
        [ObservableProperty]
        private string _targetValue = "";
    }
}
