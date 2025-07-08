using SapphireXR_App.Common;
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
        }

        private void ConfirmBeforeToggle(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Util.ConfirmBeforeToggle(sender, e);
        }
    }
}
