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
