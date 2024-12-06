using System.Windows; 
using SapphireXR_App.Views;
using SapphireXR_App.Enums;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace SapphireXR_App.ViewModels
{
    public class HomeViewModel : DependencyObject, INotifyPropertyChanged
    {
        public HomeViewModel()
        {
            SwitchValveModeCommand = new RelayCommand<object>((object? menuName) => {
                switch ((string)menuName!) {
                    case "To Single Mode":
                        SwitchableValveModeStr = toInteroperatedModeStr;
                        break;

                    case "To Interoperated Mode":
                        SwitchableValveModeStr = toSingleModeStr;
                        break;
                }
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand SwitchValveModeCommand { get; set; }

        public string SwitchableValveModeStr
        {
            set { 
                SetValue(SwitchableValveModeStrProperty, value); 
                switch(value)
                {
                case "To Single Mode":
                    SwitchableValveMode = SwitchableValveMode.Interoperation;
                    break;

                case "To Interoperated Mode":
                    SwitchableValveMode = SwitchableValveMode.Single;
                    break;
                }
                OnPropertyChanged(nameof(SwitchableValveModeStr));
            }
            get { return (string)GetValue(SwitchableValveModeStrProperty); }
        }

        public SwitchableValveMode SwitchableValveMode
        {
            get { return (SwitchableValveMode)GetValue(SwitchableValveModeProperty); }
            set { 
                SetValue(SwitchableValveModeProperty, value); 
                OnPropertyChanged(nameof(SwitchableValveMode));
            }
        }

        public string LeakTestModeStr
        {
            set { 
                SetValue(LeakTestModeStrProperty, value);
                OnPropertyChanged(nameof(LeakTestModeStr));
            }
            get { return (string)GetValue(LeakTestModeStrProperty); }
        }

        const string toInteroperatedModeStr = "To Interoperated Mode";
        const string toSingleModeStr = "To Single Mode";
        static readonly DependencyProperty SwitchableValveModeStrProperty = DependencyProperty.Register("SwitchableValveModeStr", typeof(string),
            typeof(HomePage), new PropertyMetadata(toSingleModeStr));
        static readonly DependencyProperty SwitchableValveModeProperty = DependencyProperty.Register("SwitchableValveMode", typeof(SwitchableValveMode),
            typeof(HomePage), new PropertyMetadata(SwitchableValveMode.Interoperation));


        const string enableLeakTestModeStr = "Enable Leak Test Mode";
        const string disableLeakTestModeStr = "Disable Leak Test Mode";
        static readonly DependencyProperty LeakTestModeStrProperty = DependencyProperty.Register("LeakTestModeStr", typeof(string),
            typeof(HomePage), new PropertyMetadata(enableLeakTestModeStr));

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
