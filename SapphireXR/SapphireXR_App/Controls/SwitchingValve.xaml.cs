using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Controls
{
    public partial class SwitchingValve : Valve
    {
        public SwitchingValve()
        {
            InitializeComponent();
            DataContext = new SwitchingValveViewModel();
        }
    }
}
