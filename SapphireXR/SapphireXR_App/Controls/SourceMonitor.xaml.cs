using System.Windows;
using System.Windows.Controls;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// SourceMonitor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SourceMonitor : UserControl
    {
        public SourceMonitor()
        {
            InitializeComponent();
            DataContext = new SourceMonitorViewModel();
        }
        public string MonitorID { get; set; } = "";

        private void sourceMonitor_Loaded(object sender, RoutedEventArgs e)
        {
            SourceMonitor monitor = (SourceMonitor)sender;
            sourceName.Text = monitor.MonitorID;
            ((SourceMonitorViewModel)DataContext).OnLoadedCommand.Execute(monitor.MonitorID);
        }
    }
}
