using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SapphireXR_App.Bases;
using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using SapphireXR_App.WindowServices;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SapphireXR_App.ViewModels
{
    public partial class MainViewModel : ViewModelBase, IObserver<RecipeRunViewModel.RecipeUserState>, IObserver<int>, IObserver<string>, IObserver<PLCConnection>
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

            if (PLCService.Connected == PLCConnection.Connected)
            {
                changeOperationMode(SelectedTab);
            }

            PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(SelectedTab):
                        if (PLCService.Connected == PLCConnection.Connected)
                        {
                            changeOperationMode(SelectedTab);
                        }
                        selectedTabPublisher.Issue(SelectedTab);
                        break;

                    case nameof(RecipeRunInactive):
                        if (RecipeRunInactive == true)
                        {
                            onClosing = onRecipeInactive;
                        }
                        else
                        {
                            onClosing = onRecipeActive;
                        }
                        break;
                }
            };
            onClosing = onRecipeInactive;
            ObservableManager<RecipeRunViewModel.RecipeUserState>.Subscribe("RecipeRun.State", this);
            ObservableManager<int>.Subscribe("SwitchTab", this);
            ObservableManager<string>.Subscribe("ViewModelCreated", this);
            ObservableManager<PLCConnection>.Subscribe("PLCService.Connected", this);
        }

        private void changeOperationMode(int tab)
        {
            switch (tab)
            {
                case 0:
                    PLCService.WriteOperationMode(false);
                    break;

                case 1:
                    PLCService.WriteOperationMode(true);
                    break;
            }
        }

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

        public ICommand OnClosingCommand => new RelayCommand<object?>((object? args) =>
        {
            CancelEventArgs? cancelEventArgs = args as CancelEventArgs;
            if (cancelEventArgs != null)
            {
                onClosing(cancelEventArgs);
            }

        });

        private void onRecipeInactive(CancelEventArgs args)
        {
            if (ConfirmMessage.Show("프로그램 종료", "프로그램을 종료하시겠습니까?", WindowStartupLocation.CenterScreen) == ValveOperationExResult.Ok)
            {
                closingPublisher.Issue(true);
                AppSetting.Save();
            }
            else
            {
                args.Cancel = true;
            }
        }

        private void onRecipeActive(CancelEventArgs args)
        {
            MessageBox.Show("Recipe가 실행 중인 상태에서는 애플리케이션을 종료할 수 없습니다. 현재 실행 중인 Recipe를 중지하거나 Recipe가 실행완료 될 때까지 기다리시기 바랍니다.");
            args.Cancel = true;
        }

        void IObserver<RecipeRunViewModel.RecipeUserState>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<RecipeRunViewModel.RecipeUserState>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<RecipeRunViewModel.RecipeUserState>.OnNext(RecipeRunViewModel.RecipeUserState recipeRunState)
        {
            RecipeRunInactive = !(RecipeRunViewModel.RecipeUserState.Run <= recipeRunState && recipeRunState <= RecipeRunViewModel.RecipeUserState.Pause);
        }

        void IObserver<int>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<int>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<int>.OnNext(int value)
        {
            if (value < 5 && SelectedTab != value)
            {
                SelectedTab = value;
            }
        }

        void IObserver<string>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<string>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<string>.OnNext(string value)
        {
            if(value == "RecipeRunViewModel" || value == "HomeViewModel")
            {
                ++viewmodelInterestedCreatedCount;
                if (viewmodelInterestedCreatedCount == 2)
                {
                    applicationEventIssuer.Issue(new() { Date = Util.ToEventLogFormat(App.AppStartTime), Message = "SapphireXR 시작", Type = "Application" });
                }
            }
        }

        void IObserver<PLCConnection>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<PLCConnection>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<PLCConnection>.OnNext(PLCConnection value)
        {
            switch(value)
            {
                case PLCConnection.Connected:
                    changeOperationMode(SelectedTab);
                    ToastMessage.Show("PLC로 연결되었습니다", ToastMessage.MessageType.Sucess);
                    break;

                case PLCConnection.Disconnected:
                    ToastMessage.Show("PLC로 연결이 끊겼습니다.", ToastMessage.MessageType.Error);
                    break;
            }
        }

        [ObservableProperty]
        private int _selectedTab;
        [ObservableProperty]
        private bool _recipeRunInactive = true;

        private Action<CancelEventArgs> onClosing;
        private uint viewmodelInterestedCreatedCount = 0;

        private ObservableManager<int>.Publisher selectedTabPublisher = ObservableManager<int>.Get("MainView.SelectedTabIndex");
        private ObservableManager<bool>.Publisher closingPublisher = ObservableManager<bool>.Get("App.Closing");
        private ObservableManager<EventLog>.Publisher applicationEventIssuer = ObservableManager<EventLog>.Get("EventLog");
    }
}
