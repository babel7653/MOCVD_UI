using System.Windows;
using System.Windows.Controls;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// SwitchingValve.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SwitchingValve : UserControl
    {
        public SwitchingValve()
        {
            InitializeComponent();
        }
        public string ValveID
        {
            get { return (string)GetValue(ValveIDProperty); }
            set { SetValue(ValveIDProperty, value); }
        }
        public static readonly DependencyProperty ValveIDProperty =
            DependencyProperty.Register("ValveID", typeof(string), typeof(SwitchingValve), new PropertyMetadata(default));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(SwitchingValve), new PropertyMetadata(default));

        public bool IsMaintenanceMode
        {
            get { return (bool)GetValue(IsMaintenanceModeProperty); }
            set { SetValue(IsMaintenanceModeProperty, value); }
        }

        public static readonly DependencyProperty IsMaintenanceModeProperty =
            DependencyProperty.Register("IsMaintenanceMode", typeof(bool), typeof(SwitchingValve), new PropertyMetadata(default));

    }
}
