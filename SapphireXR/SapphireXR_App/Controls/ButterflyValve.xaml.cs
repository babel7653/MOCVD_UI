using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// ButterflyValve.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ButterflyValve : UserControl
    {
        public ButterflyValve()
        {
            InitializeComponent();
        }
        public string ValveID
        {
            get { return (string)GetValue(ValveIDProperty); }
            set { SetValue(ValveIDProperty, value); }
        }
        public static readonly DependencyProperty ValveIDProperty =
            DependencyProperty.Register("ValveID", typeof(string), typeof(ButterflyValve), new PropertyMetadata(default));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(ButterflyValve), new PropertyMetadata(default));

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

        // Using a DependencyProperty as the backing store for setValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty setValueProperty =
            DependencyProperty.Register("setValue", typeof(int), typeof(ButterflyValve), new PropertyMetadata(0));

        private void ButterflyValve_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
