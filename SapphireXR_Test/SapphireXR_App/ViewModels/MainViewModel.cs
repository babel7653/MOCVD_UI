using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OxyPlot;
using SapphireXR_App.Models;
using TwinCAT.Ads;

namespace SapphireXR_App.ViewModels
{
  public class MainViewModel : ViewModelBase
  {
    public static List<string> MfcName = ["MFC01", "MFC02", "MFC03", "MFC04", "MFC05"];
    public static List<int> MfcSetValue = [1000, 2000, 3000, 4000, 5000];
    public static List<int> MfcRampTime = [100, 200, 300, 400, 500];
    public static List<int> MfcCurValue = [111, 222, 333, 444, 555];
    public static List<int> MfcDevValue = [1, 2, 3, 4, 5];
    public static List<int> MfcConValue = [11, 22, 33, 44, 55];
    public static List<int> MfcMaxValue = [1111, 2222, 3333, 4444, 5555];

    public static List<int> SetDialogValue = [4, 4000, 400, 4, 444, 44, 4444];

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
    private string _navigationSource;
    public string NavigationSource
    {
      get { return _navigationSource; }
      set { SetProperty(ref _navigationSource, value); }
    }
    // 네비게이트 커맨드
    public ICommand NavigateCommand { get; set; }

    public MainViewModel()
    {
      //Title = "SapphireXR";
      Init();
    }

    public PlotModel? PlotModel { get; set; } = default;

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
      NavigationSource = "Views/HomePage.xaml";
      NavigateCommand = new RelayCommand<string>(OnNavigate);

      //네비게이션 메시지 수신 등록
      WeakReferenceMessenger.Default.Register<NavigationMessage>(this, OnNavigationMessage);
    }


    private void OnNavigationMessage(object recipient, NavigationMessage message)
    {
      NavigationSource = message.Value;
      //OnPropertyChanged(NavigationSource);
    }
    private void OnNavigate(string? pageUri)
    {
      NavigationSource = pageUri;
      //OnPropertyChanged(NavigationSource);

    }
  }
}
