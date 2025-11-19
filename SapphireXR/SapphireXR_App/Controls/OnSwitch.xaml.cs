using SapphireXR_App.ViewModels.Valve;
using System.Windows;
using System.Windows.Controls;

namespace SapphireXR_App.Controls
{
    public partial class OnSwitch : UserControl
    {
        public OnSwitch()
        {
            InitializeComponent();
            DataContext = new OnSwitchViewModel();
        }
        public string? SwitchID { get; set; }
    }
}
