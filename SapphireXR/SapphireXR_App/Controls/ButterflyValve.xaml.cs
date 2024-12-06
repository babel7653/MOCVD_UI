using System.Windows;

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
        }
        
        public bool IsControl
        {
            get { return (bool)GetValue(IsControlProperty); }
            set { SetValue(IsControlProperty, value); }
        }

        public static readonly DependencyProperty IsControlProperty =
            DependencyProperty.Register("IsControl", typeof(bool), typeof(ButterflyValve), new PropertyMetadata(default));

        public int setValue
        {
            get { return (int)GetValue(setValueProperty); }
            set { SetValue(setValueProperty, value); }
        }

        public static readonly DependencyProperty setValueProperty =
            DependencyProperty.Register("setValue", typeof(int), typeof(ButterflyValve), new PropertyMetadata(0));

        private void ButterflyValve_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
