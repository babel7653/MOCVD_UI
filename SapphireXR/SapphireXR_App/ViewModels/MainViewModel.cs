using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OxyPlot;
using SapphireXR_App.Bases;
using SapphireXR_App.Models;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using TwinCAT.Ads;

namespace SapphireXR_App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // Connect to PLC
        public string AddressPLC { get; set; } = "PLC Address : ";
        public string ModePLC { get; set; } = "System Mode : ";

        // Variable handles to be connected plc variables
        uint hStatePLC = 0;
        public bool TcStatePLC { get; set; }

        //Create an instance of the TcAdsClient()
        public static AdsClient Ads = new();
        AmsNetId amsNetId = new("10.10.10.10.1.1");

        // 네비게이션 소스
        private string? _navigationSource;
        public string? NavigationSource
        {
            get { return _navigationSource; }
            set { SetProperty(ref _navigationSource, value); }
        }
        // 네비게이트 커맨드
        public ICommand NavigateCommand { get; set; }
        public MainViewModel()
        {
            Title = "SapphireXR";
            Init();
        }

        public PlotModel PlotModel { get; set; } = default;

        private void Init()
        {
            try
            {
                Ads.Connect(AmsNetId.Local, 851);
                if (Ads.IsConnected)
                {
                    hStatePLC = Ads.CreateVariableHandle("MAIN.StatePLC");
                    TcStatePLC = (bool)Ads.ReadAny(hStatePLC, typeof(bool));
                    AddressPLC = $"PLC Address : {Ads.Address}";
                    ModePLC = "System Mode : Ready";

                    Console.WriteLine("Connected");
                }
            }
            catch
            {
                MessageBox.Show("TwinCAT이 연결되지 않았습니다.");
                AddressPLC = "PLC Address : ";
                ModePLC = "System Mode : Not Connected";
            }

            //시작 페이지 설정
            NavigationSource = "Views/RecipeRunPage.xaml";
            NavigateCommand = new RelayCommand<string>(OnNavigate);

            //네비게이션 메시지 수신 등록
            WeakReferenceMessenger.Default.Register<NavigationMessage>(this, OnNavigationMessage);
        }
        private void OnNavigationMessage(object recipient, NavigationMessage message)
        {
            NavigationSource = message.Value;
        }
        private void OnNavigate(string pageUri)
        {
            NavigationSource = pageUri;
        }
    }
}
