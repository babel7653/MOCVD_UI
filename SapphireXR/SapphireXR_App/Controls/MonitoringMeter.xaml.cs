using SapphireXR_App.ViewModels;
using System.Windows.Controls;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// MonitoringMeter.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MonitoringMeter : UserControl
    {
        public MonitoringMeter()
        {
            InitializeComponent();
            DataContext = new MonitoringMeterViewModel();
        }
        public string? Type { get; set; }
    }
}
