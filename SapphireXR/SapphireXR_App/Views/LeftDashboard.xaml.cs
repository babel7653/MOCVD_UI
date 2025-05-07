using SapphireXR_App.ViewModels;
using System.Windows.Controls;

namespace SapphireXR_App.Views
{
    public partial class LeftDashboard : Page
    {
        public LeftDashboard()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(LeftViewModel));
        }

        private void tbShowerHeadTemp_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
