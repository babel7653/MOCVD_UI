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
    /// SingleValve.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SingleValve : UserControl
    {
        public SingleValve()
        {
            InitializeComponent();
        }
        public string ValveID
        {
            get { return (string)GetValue(ValveIDProperty); }
            set { SetValue(ValveIDProperty, value); }
        }
        public static readonly DependencyProperty ValveIDProperty =
            DependencyProperty.Register("ValveID", typeof(string), typeof(SingleValve), new PropertyMetadata(default));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(SingleValve), new PropertyMetadata(default));

        public bool IsNormallyOpen
        {
            get { return (bool)GetValue(IsNormallyOpenProperty); }
            set { SetValue(IsNormallyOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsNormallyOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNormallyOpenProperty =
            DependencyProperty.Register("IsNormallyOpen", typeof(bool), typeof(SingleValve), new PropertyMetadata(default));

        private void SingleValve_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}