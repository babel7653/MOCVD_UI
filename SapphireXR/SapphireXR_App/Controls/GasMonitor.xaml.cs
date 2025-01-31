using System.Windows;
using System.Windows.Controls;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// GasMonitor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GasMonitor : UserControl
    {
        public GasMonitor()
        {
            InitializeComponent();
            DataContext = new GasMonitorViewModel();
        }

        public string MonitorID { get; set; } = "";

        private void gasMonitor_Loaded(object sender, RoutedEventArgs e)
        {
            GasMonitor monitor = (GasMonitor)sender;
            gasName.Text = monitor.MonitorID;
            ((GasMonitorViewModel)DataContext).OnLoadedCommand.Execute(monitor.MonitorID);
        }
    }
}
