using SapphireXR_App.Controls;
using SapphireXR_App.Models;
using SapphireXR_App.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;



namespace SapphireXR_App.Views
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(HomeViewModel));
        }

        private void BlockValve_Click(object sender, MouseButtonEventArgs e)
        {
            BlockValve blockValve = (BlockValve)sender;
            ValveOperationEx.Show("Valve Operation", " 질소가스로 변경하시겠습니까?");
        }
    }
}
