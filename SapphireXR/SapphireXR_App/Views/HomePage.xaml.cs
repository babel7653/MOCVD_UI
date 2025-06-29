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
            ToggleButton? toggleSwitch = sender as ToggleButton;
            if (toggleSwitch != null)
            {
                string destState = toggleSwitch.IsChecked == true ? "On" : "Off";
                if (ValveOperationEx.Show("", "상태로 변경하시겠습니까?") == ValveOperationExResult.Cancel)
                {
                    e.Handled = true;
                }
                else
                {
                    toggleSwitch.IsChecked = !toggleSwitch.IsChecked;
                    toggleSwitch.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
        }
    }
}