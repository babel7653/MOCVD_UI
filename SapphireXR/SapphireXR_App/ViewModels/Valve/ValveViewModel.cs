using System.ComponentModel;
using SapphireXR_App.Common;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using SapphireXR_App.WindowServices;

namespace SapphireXR_App.ViewModels
{
    public abstract class ValveViewModel : DependencyObject, INotifyPropertyChanged
    {
        internal abstract class ValveStateUpdater
        {
            internal ValveStateUpdater(ValveViewModel valveViewModel) 
            {
                viewModel = valveViewModel;
            }

            public abstract void OnValveClicked();

            protected ValveViewModel viewModel;
        }

        internal class ValveStateUpdaterFromCurrentPLCState: ValveStateUpdater, IObserver<bool>
        {
            internal ValveStateUpdaterFromCurrentPLCState(ValveViewModel valveViewModel): base(valveViewModel) 
            {
                if (viewModel.ValveID != null)
                {
                    string valveStateTopicName = "Valve.OnOff." + valveViewModel.ValveID + ".CurrentPLCState";
                    ObservableManager<bool>.Subscribe(valveStateTopicName, this);
                }
            }

            public override void OnValveClicked()
            {
                if (popUpMessage == null)
                {
                    popUpMessage = viewModel.getPopupMessage();
                }
                PopupMessage actual = popUpMessage.Value;
                string valveOperationMessage = (viewModel.IsOpen == true ? actual.messageWithOpen : actual.messageWithoutOpen);
                string confirmMessage = (viewModel.IsOpen == true ? actual.confirmWithOpen : actual.confirmWithoutOpen);
                string cancelMessage = (viewModel.IsOpen == true ? actual.cancelWithOpen : actual.cancelWithoutOpen);
                var result = ValveOperationEx.Show("Valve Operation", valveOperationMessage);
                switch (result)
                {
                    case DialogResult.Ok:
                        bool isOpen = !(viewModel.IsOpen);
                        if (viewModel.ValveID != null)
                        {
                            try
                            {
                                PLCService.WriteValveState(viewModel.ValveID, isOpen);
                            }
                            catch (Exception exception)
                            {
                                ToastMessage.Show("PLC로 " + viewModel.ValveID + "값을 쓰는데 실패했습니다. 원인은 다음과 같습니다: " + exception.Message, ToastMessage.MessageType.Error);
                                return;
                            }

                            viewModel.IsOpen = isOpen;
                            ToastMessage.Show(confirmMessage, ToastMessage.MessageType.Sucess);
                        }
                        break;

                    case DialogResult.Cancel:
                        ToastMessage.Show(cancelMessage, ToastMessage.MessageType.Information);
                        break;
                }
            }

            void IObserver<bool>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<bool>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<bool>.OnNext(bool value)
            {
                if (value != viewModel.IsOpen)
                {
                    viewModel.IsOpen = value;
                }
            }

            private PopupMessage? popUpMessage;
        }

        internal class ValveStateUpdaterFromCurrentRecipeStep : ValveStateUpdater, IObserver<bool>
        {
            private class ResetValveStateSubscriber : IObserver<bool>
            {
                public ResetValveStateSubscriber(ValveViewModel vm)
                {
                    valveViewModel = vm;
                }

                public void OnCompleted()
                {
                    throw new NotImplementedException();
                }

                public void OnError(Exception error)
                {
                    throw new NotImplementedException();
                }

                public void OnNext(bool value)
                {
                    valveViewModel.IsOpen = false;
                }

                ValveViewModel valveViewModel;
            }
            internal ValveStateUpdaterFromCurrentRecipeStep(ValveViewModel valveViewModel) : base(valveViewModel) 
            {
                string topicName = "Valve.OnOff." + valveViewModel.ValveID + ".CurrentRecipeStep";
                isOpenChangedPubisher = ObservableManager<bool>.Get(topicName);
                ObservableManager<bool>.Subscribe(topicName, this);
                ObservableManager<bool>.Subscribe("Reset.CurrentRecipeStep", resetValveStateSubscriber = new ResetValveStateSubscriber(valveViewModel));
            }

            public override void OnValveClicked()
            {
                bool isOpen = !(viewModel.IsOpen);
                viewModel.IsOpen = isOpen;
                isOpenChangedPubisher.Publish(isOpen);
            }

            void IObserver<bool>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<bool>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<bool>.OnNext(bool value)
            {
                if(viewModel.IsOpen != value)
                {
                    viewModel.IsOpen = value;
                }
            }

            ObservableManager<bool>.Publisher isOpenChangedPubisher;
            ResetValveStateSubscriber resetValveStateSubscriber;
        }

        private class PLCConnectionStateSubscriber : IObserver<PLCConnection>
        {
            public PLCConnectionStateSubscriber(ValveViewModel vm)
            {
                valveViewModel = vm;
            }

            void IObserver<PLCConnection>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<PLCConnection>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<PLCConnection>.OnNext(PLCConnection value)
            {
                valveViewModel.OnClickCommand.NotifyCanExecuteChanged();
            }

            private ValveViewModel valveViewModel;
        }

        static internal ValveStateUpdater? CreateValveStateUpdater(Controls.Valve.UpdateTarget target, ValveViewModel viewModel)
        {
            switch (target)
            {
                case Controls.Valve.UpdateTarget.CurrentPLCState:
                    return new ValveStateUpdaterFromCurrentPLCState(viewModel);

                case Controls.Valve.UpdateTarget.CurrentRecipeStep:
                    return new ValveStateUpdaterFromCurrentRecipeStep(viewModel);

                default:
                    return null;
            }
        }

        public ValveViewModel()
        {
            OnLoadedCommand = new RelayCommand<object?>((object? args) =>
            {
                if (args != null)
                {
                    object[] argArray = (object[])args;
                    if (argArray[0] is string && argArray[1] is Controls.Valve.UpdateTarget)
                    {
                        Init((string)argArray[0], (Controls.Valve.UpdateTarget)argArray[1]);
                    }
                }
            });
        }

        protected virtual void Init(string valveID, Controls.Valve.UpdateTarget target)
        {
            ValveID = valveID;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand OnLoadedCommand { get; set; }
        public RelayCommand OnClickCommand => new RelayCommand(OnClicked, () => PLCConnectionState.Instance.Online == true);

        protected abstract void OnClicked();

        public string? ValveID { get; set; }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(ValveViewModel), new PropertyMetadata(default));

        protected struct PopupMessage
        {
            public string messageWithOpen;
            public string confirmWithOpen;
            public string cancelWithOpen;
            public string messageWithoutOpen;
            public string confirmWithoutOpen;
            public string cancelWithoutOpen;
        };
        protected virtual PopupMessage getPopupMessage()
        {
            return new PopupMessage();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
