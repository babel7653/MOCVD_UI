﻿using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using SapphireXR_App.Enums;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using static SapphireXR_App.ViewModels.FlowControlViewModel;
using CommunityToolkit.Mvvm.ComponentModel;

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
                    if (controlValues.targetValue != null)
                    {
                        PLCService.WriteTargetValue(ControllerID, (int)controlValues.targetValue);
                    }
                    if (controlValues.rampTime != null)
                    {
                        PLCService.WriteRampTime(ControllerID, (short)controlValues.rampTime);
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
            selectedThis?.Issue(ControllerID);
        }

        protected ControlTargetValueSubscriber? controlTargetValueSubscriber;
        private ObservableManager<string>.DataIssuer? selectedThis;

        [ObservableProperty]
        private string _currentValue = "";
        [ObservableProperty]
        private string _targetValue = "";
    }
}
