using System.Windows;
using System.Windows.Controls;

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
        }

        public string MonitorID
        {
            get { return (string)GetValue(MonitorIDProperty); }
            set { SetValue(MonitorIDProperty, value); }
        }

        public static readonly DependencyProperty MonitorIDProperty =
            DependencyProperty.Register("MonitorID", typeof(string), typeof(GasMonitor), new PropertyMetadata(default));

        private void gasMonitor_Loaded(object sender, RoutedEventArgs e)
        {
            GasMonitor monitor = (GasMonitor)sender;
            gasName.Text = monitor.MonitorID;
        }
    }
}
