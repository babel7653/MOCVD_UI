using SapphireXR_App.Common;
using SapphireXR_App.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

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

        private void DataGridColumnHeader_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DataGridColumnHeader? dataGridColumnHeader = sender as DataGridColumnHeader;
            if (dataGridColumnHeader != null)
            {
                
            }
        }
    }
}
