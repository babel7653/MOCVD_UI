using System.ComponentModel;
using SapphireXR_App.Common;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace SapphireXR_App.ViewModels
{
    public class ValveViewModel : DependencyObject, INotifyPropertyChanged, IObserver<bool>
    {
        protected virtual void OnLoaded(string? valveID)
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
