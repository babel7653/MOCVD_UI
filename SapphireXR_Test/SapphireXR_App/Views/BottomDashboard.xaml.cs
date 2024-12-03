using SapphireXR_App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
