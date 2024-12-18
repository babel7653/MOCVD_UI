using System.Windows.Controls;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// ButterflyValve.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ButterflyValve : Valve
    {
        public ButterflyValve()
        {
            InitializeComponent();
            DataContext = new ButterflyValveViewModel();
        }
    }
}
