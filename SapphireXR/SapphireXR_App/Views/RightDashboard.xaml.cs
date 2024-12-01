using SapphireXR_App.ViewModels;
using System.Windows.Controls;

namespace SapphireXR_App.Views
{
    public partial class RightDashboard : Page
    {
        public RightDashboard()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(RightViewModel));
        }
    }
}
