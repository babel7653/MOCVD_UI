﻿using CommunityToolkit.Mvvm.Input;
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
                Func<float, int> numberDecimalDigits = (float value) =>
                {
                    int intValue = (int)value;
                    if (0 <= intValue && intValue < 10)
                    {
                        return MaxNumberDigit - 1;
                    }
                    else if (10 <= intValue && intValue < 100)
                    {
                        return MaxNumberDigit - 2;
                    }
                    else if (100 <= intValue && intValue < 1000)
                    {
                        return MaxNumberDigit - 3;
                    }
                    else
                    {
                        return 0;
                    }
                };
                string controlValue = values.Item1.ToString("N", new NumberFormatInfo() { NumberDecimalDigits = numberDecimalDigits(values.Item1) });
                if(controlValue != flowControllerViewModel.ControlValue)
                {
                    flowControllerViewModel.ControlValue = controlValue;
                }
                string targetValue = values.Item2.ToString("N", new NumberFormatInfo() { NumberDecimalDigits = numberDecimalDigits(values.Item2) });
                if (targetValue != flowControllerViewModel.TargetValue)
                {
                    flowControllerViewModel.TargetValue = targetValue;
                }
            }

            private static readonly int MaxNumberDigit = 4;
            private HomeFlowControllerViewModel flowControllerViewModel;
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
