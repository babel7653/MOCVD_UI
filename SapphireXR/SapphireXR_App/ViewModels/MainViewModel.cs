using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OxyPlot;
using SapphireXR_App.Bases;
using SapphireXR_App.Models;

namespace SapphireXR_App.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string? navigationSource;

        public MainViewModel()
        {
            Title = "SapphireXR";

             //시작 페이지 설정
            NavigationSource = "Views/RecipeRunPage.xaml";
  
            //네비게이션 메시지 수신 등록
            WeakReferenceMessenger.Default.Register<NavigationMessage>(this, OnNavigationMessage);
        }

        public PlotModel PlotModel { get; set; } = default;

        private void OnNavigationMessage(object recipient, NavigationMessage message)
        {
            NavigationSource = message.Value;
        }

        // 네비게이트 커맨드
        [RelayCommand]
        private void OnNavigate(string pageUri)
        {
            NavigationSource = pageUri;
        }
    }
}
