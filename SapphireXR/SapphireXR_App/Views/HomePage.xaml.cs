using System.Windows;
using System.Windows.Controls;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Views
{
  /// <summary>
  /// HomePage.xaml에 대한 상호 작용 논리
  /// </summary>``
  public partial class HomePage : Page
  {
    public HomePage()
    {
      InitializeComponent();

      DataContext = App.Current.Services.GetService(typeof(HomeViewModel));

    }

    private void SourceControl_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      AnalogDeviceControl MfcControl = new();
      MfcControl.Show();
    }
  }
}
