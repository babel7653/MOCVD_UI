using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Interfaces;

namespace SapphireXR_App.Bases
{
    public abstract class ViewModelBase : ObservableObject, INavigationAware
    {
        private string? _title;
        // 타이틀
        public string? Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string? _message;
        // 메시지
        public string? Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        // 네비게이션 완료시
        public virtual void OnNavigated(object sender, object navigatedEventArgs)
        {
        }
        // 네비게이션 시작시
        public virtual void OnNavigating(object sender, object navigationEventArgs)
        {
        }
    }
}
