using System;
using System.ComponentModel;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Services
{
  public class MainNavigationStore : ViewModelBase
  {
    private INotifyPropertyChanged? _currentViewModel;

    public INotifyPropertyChanged? CurrentViewModel
    {
      get { return _currentViewModel; }
      set {
        _currentViewModel = value;
        CurrentViewModelChanged?.Invoke();
        _currentViewModel = null;
      }
    }

    public Action? CurrentViewModelChanged { get; set; }
  }
}
