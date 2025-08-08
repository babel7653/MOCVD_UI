using SapphireXR_App.Common;
using SapphireXR_App.ViewModels;
using System.Windows.Controls;

namespace SapphireXR_App.Views
{
    public partial class HomePage : Page
    {
        public static bool IsMaintenanceMode { get; set; } = false;
        public HomePage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(HomeViewModel));
        }

        private void ConfirmBeforeToggle(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Util.ConfirmBeforeToggle(sender, e);
        }
    }
}