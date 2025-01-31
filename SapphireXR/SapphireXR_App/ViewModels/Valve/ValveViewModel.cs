using System.ComponentModel;
using SapphireXR_App.Common;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using SapphireXR_App.Controls;
using static SapphireXR_App.ViewModels.ValveViewModel;

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

        internal class ValveStateUpdaterFromCurrentPLCState: ValveStateUpdater
        {
            internal ValveStateUpdaterFromCurrentPLCState(ValveViewModel valveViewModel): base(valveViewModel) 
            {
                if (viewModel.ValveID != null)
                {
                    viewModel.IsOpen = PLCService.ReadValveState(viewModel.ValveID);
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
                    case ValveOperationExResult.Ok:
                        bool isOpen = !(viewModel.IsOpen);
                        viewModel.IsOpen = isOpen;
                        if (viewModel.ValveID != null)
                        {
                            PLCService.WriteValveState(viewModel.ValveID, isOpen);
                        }
                        MessageBox.Show(confirmMessage);
                        break;

                    case ValveOperationExResult.Cancel:
                        MessageBox.Show(cancelMessage);
                        break;
                }
            }

            private PopupMessage? popUpMessage;
        }

        internal class ValveStateUpdaterFromCurrentRecipeStep : ValveStateUpdater, IObserver<(object, bool)>, IObserver<bool>
        {
            internal ValveStateUpdaterFromCurrentRecipeStep(ValveViewModel valveViewModel) : base(valveViewModel) 
            {
                string topicName = "Valve.OnOff." + valveViewModel.ValveID + ".CurrentRecipeStep";
                isOpenChangedPubisher = ObservableManager<(object, bool)>.Get(topicName);
                ObservableManager<(object, bool)>.Subscribe(topicName, this);
                ObservableManager<bool>.Subscribe("Reset.CurrentRecipeStep", this);
            }

            public override void OnValveClicked()
            {
                bool isOpen = !(viewModel.IsOpen);
                viewModel.IsOpen = isOpen;
                isOpenChangedPubisher.Issue((this, isOpen));
            }

            void IObserver<(object, bool)>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<(object, bool)>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<(object, bool)>.OnNext((object, bool) value)
            {
                if(value.Item1 != this && viewModel.IsOpen != value.Item2)
                {
                    viewModel.IsOpen = value.Item2;
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
                viewModel.IsOpen = false;
            }

            ObservableManager<(object, bool)>.DataIssuer isOpenChangedPubisher;
        }

        static internal ValveStateUpdater? CreateValveStateUpdater(SapphireXR_App.Controls.Valve.UpdateTarget target, ValveViewModel viewModel)
        {
            switch (target)
            {
                case SapphireXR_App.Controls.Valve.UpdateTarget.CurrentPLCState:
                    return new ValveStateUpdaterFromCurrentPLCState(viewModel);

                case SapphireXR_App.Controls.Valve.UpdateTarget.CurrentRecipeStep:
                    return new ValveStateUpdaterFromCurrentRecipeStep(viewModel);

                default:
                    return null;
            }
        }

        protected virtual void Init(string valveID, SapphireXR_App.Controls.Valve.UpdateTarget target)
        {
            ValveID = valveID;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand OnLoadedCommand => new RelayCommand<object?>((object? args) =>
        {
            if (args != null)
            {
                object[] argArray = (object[])args;
                if (argArray[0] is string && argArray[1] is SapphireXR_App.Controls.Valve.UpdateTarget)
                {
                    Init((string)argArray[0], (SapphireXR_App.Controls.Valve.UpdateTarget)argArray[1]);
                }
            }
        });
        public ICommand OnClickCommand => new RelayCommand(OnClicked);

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
