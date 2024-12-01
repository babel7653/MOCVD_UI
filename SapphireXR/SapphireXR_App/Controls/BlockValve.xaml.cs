using System.Windows;
using System.Windows.Controls;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// BlockValve.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BlockValve : UserControl
    {
        public BlockValve()
        {
            InitializeComponent();
        }
        public string ValveID
        {
            get { return (string)GetValue(ValveIDProperty); }
            set { SetValue(ValveIDProperty, value); }
        }
        public static readonly DependencyProperty ValveIDProperty =
            DependencyProperty.Register("ValveID", typeof(string), typeof(BlockValve), new PropertyMetadata(default));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(BlockValve), new PropertyMetadata(default));
    }
}
