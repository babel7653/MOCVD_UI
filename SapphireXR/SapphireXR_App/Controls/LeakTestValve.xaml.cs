using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// LeakTestValve.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LeakTestValve : Valve
    {
        public LeakTestValve()
        {
            InitializeComponent();
            DataContext = new LeakTestValveViewModel();
        }
    }
}