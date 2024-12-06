using System.Windows;
using System.Windows.Controls;

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
        }
        public string MonitorID
        {
            get { return (string)GetValue(MonitorIDProperty); }
            set { SetValue(MonitorIDProperty, value); }
        }

        public static readonly DependencyProperty MonitorIDProperty =
            DependencyProperty.Register("MonitorID", typeof(string), typeof(SourceMonitor), new PropertyMetadata(default));

        private void sourceMonitor_Loaded(object sender, RoutedEventArgs e)
        {
            SourceMonitor monitor = (SourceMonitor)sender;
            sourceName.Text = monitor.MonitorID;
        }
    }
}
