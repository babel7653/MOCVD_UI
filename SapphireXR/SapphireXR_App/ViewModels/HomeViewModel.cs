using System.Windows; 
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Collections;

namespace SapphireXR_App.ViewModels
{
    public class HomeViewModel : DependencyObject, INotifyPropertyChanged
    {
        public static uint[]? aSolValvePLC { get; set; }
        public static uint hValve { get; set; }
        public static BitArray bValveState { get; set; }
        public HomeViewModel()
        {
            EnableLeakTestCommand = new RelayCommand<object>((object? menuName) => {
                switch ((string)menuName!) {
                    case "Enable Leak Test Mode":
                        OnLeakTestVisibility = Visibility.Visible;
                        OffLeakTestVisibility = Visibility.Hidden;
                        LeakTestModeStr = disableLeakTestModeStr;
                        break;

                    case "Disable Leak Test Mode":
                        OnLeakTestVisibility = Visibility.Hidden;
                        OffLeakTestVisibility = Visibility.Visible;
                        LeakTestModeStr = enableLeakTestModeStr;
                        break;
                }
            });
            ReadSolValveState(); // 초기 로드시 PLC Valve상태 읽음

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand EnableLeakTestCommand { get; set; }

        public string LeakTestModeStr
        {
            set {
                SetValue(LeakTestModeStrProperty, value);
                OnPropertyChanged(nameof(LeakTestModeStr));
            }
            get { return (string)GetValue(LeakTestModeStrProperty); }
        }

        const string enableLeakTestModeStr = "Enable Leak Test Mode";
        const string disableLeakTestModeStr = "Disable Leak Test Mode";
        static readonly DependencyProperty LeakTestModeStrProperty = DependencyProperty.Register("LeakTestModeStr", typeof(string),
            typeof(HomeViewModel), new PropertyMetadata(enableLeakTestModeStr));

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

        private void ReadSolValveState()
        {
            // Solenoid Valve State Read (Update)
            try
            {
                hValve = MainViewModel.Ads.CreateVariableHandle("GVL_IO.aOutputSolValve");
                aSolValvePLC = (uint[])MainViewModel.Ads.ReadAny(hValve, typeof(uint[]), new int[] { 2 });
                //bValveState =  
                Console.WriteLine("aSolValvePLC");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
