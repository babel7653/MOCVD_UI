using System.ComponentModel;
using SapphireXR_App.Common;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;

namespace SapphireXR_App.ViewModels
{
    public class ValveViewModel : DependencyObject, INotifyPropertyChanged, IObserver<bool>
    {
        protected virtual void Init(string? valveID)
        {
            ValveID = valveID;
            try
            {
                isOpenValueChanged = ObservableManager<bool>.Get(ValveID + ".IsOpen.Write");
            }
            catch (ObservableManager<bool>.DataIssuerBaseCreateException)
            {
            }
            ObservableManager<bool>.Subscribe(ValveID + ".IsOpen.Read", this);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand OnLoadedCommand => new RelayCommand<string>(Init);
        public ICommand OnClickCommand => new RelayCommand(OnClicked);

        protected virtual void OnClicked()
        {
            if (popUpMessage == null)
            {
                popUpMessage = getPopupMessage();
            }
            PopupMessage actual = popUpMessage.Value;
            string valveOperationMessage = (IsOpen == true ? actual.messageWithOpen : actual.messageWithoutOpen);
            string confirmMessage = (IsOpen == true ? actual.confirmWithOpen : actual.confirmWithoutOpen);
            string cancelMessage = (IsOpen == true ? actual.cancelWithOpen : actual.cancelWithoutOpen);
            var result = ValveOperationEx.Show("Valve Operation", valveOperationMessage);
            switch (result)
            {
                case ValveOperationExResult.Ok:
                    IsOpenObservable = !(IsOpen);
                    MessageBox.Show(confirmMessage);
                    //TODO

                    break;
                case ValveOperationExResult.Cancel:
                    MessageBox.Show(cancelMessage);
                    break;
            }
        }

        public string? ValveID { get; set; }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set
            {
                SetValue(IsOpenProperty, value);
                OnPropertyChanged(nameof(IsOpen));
            }
        }

        //PLC에 변경값을 쓰려면 IsOpen 대신 이 프로퍼티를 사용
        public bool IsOpenObservable
        {
            set
            {
                IsOpen = value;
                isOpenValueChanged?.Issue(value);
            }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(ValveViewModel), new PropertyMetadata(default));

        private ObservableManager<bool>.DataIssuerBase? isOpenValueChanged;

        protected struct PopupMessage
        {
            public string messageWithOpen;
            public string confirmWithOpen;
            public string cancelWithOpen;
            public string messageWithoutOpen;
            public string confirmWithoutOpen;
            public string cancelWithoutOpen;
        };
        private PopupMessage? popUpMessage;

        protected virtual PopupMessage getPopupMessage()
        {
            return new PopupMessage();
        }

        void IObserver<bool>.OnCompleted()
        {
        }

        void IObserver<bool>.OnError(Exception error)
        {
        }

        void IObserver<bool>.OnNext(bool value)
        {
            IsOpen = value;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
