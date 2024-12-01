using SapphireXR_App.ViewModels;
using System.Windows.Controls;

namespace SapphireXR_App.Views
{
    public partial class BottomDashboard : Page
    {
        public BottomDashboard()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(BottomViewModel));
        }
    }
}
