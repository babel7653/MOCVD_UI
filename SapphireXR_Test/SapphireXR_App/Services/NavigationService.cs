using System.ComponentModel;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Services
{
  public class NavigationService : INavigationService
  {
    private readonly MainNavigationStore _mainNavigationStore;
    private INotifyPropertyChanged CurrentViewModel
    {
      set => _mainNavigationStore.CurrentViewModel = value;
    }

    public NavigationService(MainNavigationStore mainNavigationStore)
    {
      this._mainNavigationStore = mainNavigationStore;
    }

    public void Navigate(NaviType naviType)
    {
      switch (naviType)
      { 
        case NaviType.HomePage:
          CurrentViewModel = (ViewModelBase)App.Current.Services.GetService(typeof(HomeViewModel))!;
          break;
        case NaviType.RecipePage:
          CurrentViewModel = (ViewModelBase)App.Current.Services.GetService(typeof(RecipeViewModel))!;
          break;
        case NaviType.ReportPage:
          CurrentViewModel = (ViewModelBase)App.Current.Services.GetService(typeof(ReportViewModel))!;
          break;
        case NaviType.SettingPage:
          CurrentViewModel = (ViewModelBase)App.Current.Services.GetService(typeof(SettingViewModel))!;
          break;
        default:
          return;
      }
    }
  }
}
