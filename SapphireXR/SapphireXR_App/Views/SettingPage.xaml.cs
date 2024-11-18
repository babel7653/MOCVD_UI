using SapphireXR_App.Models;
using SapphireXR_App.ViewModels;
using System.Windows.Controls;

namespace SapphireXR_App.Views
{
  public partial class SettingPage : Page
    {
        public SettingPage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(SettingViewModel));

            //comboSystemStart.ItemsSource = Enum.GetValues(typeof(EUserState)).Cast<EUserState>();
            //comboAlarmStart.ItemsSource = Enum.GetValues(typeof(EUserState)).Cast<EUserState>();
            //comboRecipeEnd.ItemsSource = Enum.GetValues(typeof(EUserState)).Cast<EUserState>();
        }
    }
}
