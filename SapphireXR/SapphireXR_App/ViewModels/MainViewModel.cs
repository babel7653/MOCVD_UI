using Caliburn.Micro;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OxyPlot;
using SapphireXR_App.Bases;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using System.ComponentModel;
using System.Windows.Input;

namespace SapphireXR_App.ViewModels
{
    public partial class MainViewModel : ViewModelBase, IObserver<RecipeRunViewModel.RecipeRunState>
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

            PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
            {
                switch(args.PropertyName)
                {
                    case nameof(SelectedTab):
                        selectedTabPublisher.Issue(SelectedTab);
                        break;
                }
            };
            ObservableManager<RecipeRunViewModel.RecipeRunState>.Subscribe("RecipeRun.State", this);
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

        public ICommand OnClosingCommand => new RelayCommand(() =>
        {

        });

        void IObserver<RecipeRunViewModel.RecipeRunState>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<RecipeRunViewModel.RecipeRunState>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<RecipeRunViewModel.RecipeRunState>.OnNext(RecipeRunViewModel.RecipeRunState recipeRunState)
        {
            RecipeRunInactive = !(RecipeRunViewModel.RecipeRunState.Run <= recipeRunState && recipeRunState < RecipeRunViewModel.RecipeRunState.Restart);
        }

        [ObservableProperty]
        private int _selectedTab;
        [ObservableProperty]
        private bool _recipeRunInactive = true;

        private ObservableManager<int>.DataIssuer selectedTabPublisher = ObservableManager<int>.Get("MainView.SelectedTabIndex");

    }
}
