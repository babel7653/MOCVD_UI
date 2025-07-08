using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using SapphireXR_App.ViewModels;
using SapphireXR_App.WindowServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

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