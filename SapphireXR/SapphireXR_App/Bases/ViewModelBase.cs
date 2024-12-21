using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Interfaces;

namespace SapphireXR_App.Bases
{
    public abstract partial class ViewModelBase : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private string? title;
        [ObservableProperty]
        private string? message;

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
