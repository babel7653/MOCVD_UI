using System.Windows.Controls;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// SingleValve.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SingleValve : Valve
    {
        public SingleValve()
        {
            InitializeComponent();
            DataContext = new SingleValveViewModel();
        }
    }
}