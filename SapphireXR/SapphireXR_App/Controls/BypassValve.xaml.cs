using SapphireXR_App.ViewModels;
using System.Windows.Controls;


namespace SapphireXR_App.Controls
{
    public partial class BypassValve : Valve
    {
        public BypassValve()
        {
            InitializeComponent();
            DataContext = new BypassViewModel();
        }
    }
}
