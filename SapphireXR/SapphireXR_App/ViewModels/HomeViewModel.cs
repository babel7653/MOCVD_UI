using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SapphireXR_App.ViewModels
{
    public class HomeViewModel : DependencyObject, INotifyPropertyChanged
    {
        public HomeViewModel()
        {
            EnableLeakTestCommand = new RelayCommand<object>((object? menuName) =>
            {
                switch ((string)menuName!)
                {
                    case "Show Leak Test Valve":
                        OnLeakTestVisibility = Visibility.Visible;
                        OffLeakTestVisibility = Visibility.Hidden;
                        LeakTestModeStr = disableLeakTestModeStr;
                        break;

                    case "Hide Leak Test Mode":
                        OnLeakTestVisibility = Visibility.Hidden;
                        OffLeakTestVisibility = Visibility.Visible;
                        LeakTestModeStr = enableLeakTestModeStr;
                        break;
                }
            });

            ShowValveLabelCommand = new RelayCommand<object>((object? menuName) =>
            {
                switch ((string)menuName!)
                {
                    case "Show Label":
                        ValveLabelVisibility = Visibility.Visible;
                        ShowValveLabelStr = hideValveLabelStr;
                        break;
                    case "Hide Label":
                        ValveLabelVisibility = Visibility.Hidden;
                        ShowValveLabelStr = showValveLabelStr;
                        break;
                }
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand EnableLeakTestCommand { get; set; }
        public ICommand ShowValveLabelCommand { get; set; }

        public string LeakTestModeStr
        {
            set {
                SetValue(LeakTestModeStrProperty, value);
                OnPropertyChanged(nameof(LeakTestModeStr));
            }
            get { return (string)GetValue(LeakTestModeStrProperty); }
        }

        const string enableLeakTestModeStr = "Show Leak Test Valve";
        const string disableLeakTestModeStr = "Hide Leak Test Mode";
        static readonly DependencyProperty LeakTestModeStrProperty = DependencyProperty.Register("LeakTestModeStr", typeof(string),
            typeof(HomeViewModel), new PropertyMetadata(enableLeakTestModeStr));

        public string ShowValveLabelStr
        {
            get { return (string)GetValue(ShowValveLabelStrProperty); }
            set
            {
                SetValue(ShowValveLabelStrProperty, value);
                OnPropertyChanged(nameof(ShowValveLabelStr));
            }
        }
        const string showValveLabelStr = "Show Label";
        const string hideValveLabelStr = "Hide Label";
        public static readonly DependencyProperty ShowValveLabelStrProperty =
            DependencyProperty.Register("ShowValveLabelStr", typeof(string), typeof(HomeViewModel), new PropertyMetadata(hideValveLabelStr));

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        Visibility OnLeakTestVisibility
        {
            get { return (Visibility)GetValue(OnLeakTestVisibilityProperty); }
            set
            {
                SetValue(OnLeakTestVisibilityProperty, value);
                OnPropertyChanged(nameof(OnLeakTestVisibility));
            }
        }
        static readonly DependencyProperty OnLeakTestVisibilityProperty = DependencyProperty.Register("OnLeakTestVisibility", typeof(Visibility),
           typeof(HomeViewModel), new PropertyMetadata(Visibility.Hidden));

        Visibility OffLeakTestVisibility
        {
            get { return (Visibility)GetValue(OffLeakTestVisibilityProperty); }
            set
            {
                SetValue(OffLeakTestVisibilityProperty, value);
                OnPropertyChanged(nameof(OffLeakTestVisibility));
            }
        }
        static readonly DependencyProperty OffLeakTestVisibilityProperty = DependencyProperty.Register("OffLeakTestVisibility", typeof(Visibility),
           typeof(HomeViewModel), new PropertyMetadata(Visibility.Visible));

        Visibility ValveLabelVisibility
        {
            get { return (Visibility)GetValue(ValveLabelVisibilityProperty); }
            set { SetValue(ValveLabelVisibilityProperty, value);
            OnPropertyChanged(nameof (ValveLabelVisibility));
            }
        }
        public static readonly DependencyProperty ValveLabelVisibilityProperty =
            DependencyProperty.Register("ValveLabelVisibility", typeof(Visibility), typeof(HomeViewModel), new PropertyMetadata(Visibility.Visible));
    }
}
