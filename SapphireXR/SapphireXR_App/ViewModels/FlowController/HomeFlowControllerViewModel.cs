using System.ComponentModel;
using System.Windows;
using SapphireXR_App.Controls;
using SapphireXR_App.Views;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using System.Reactive;
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
        protected class ControlTargetValueSubscriber : IObserver<(int, int)>
        {
            public ControlTargetValueSubscriber(HomeFlowControllerViewModel viewModel)
            {
                flowControllerViewModel = viewModel;
            }
            void IObserver<(int, int)>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<(int, int)>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<(int, int)>.OnNext((int, int) values)
            {
                flowControllerViewModel.ControlValue = values.Item1.ToString();
                flowControllerViewModel.TargetValue = values.Item2.ToString();
            }

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
            ObservableManager<(int, int)>.Subscribe("FlowControl." + ControllerID + ".ControlTargetValue.CurrentPLCState", controlTargetValueSubscriber = new ControlTargetValueSubscriber(this));
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
