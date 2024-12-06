using SapphireXR_App.Enums;
using SapphireXR_App.Models;
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
    public partial class SingleValve : Valve
    {
        public SingleValve()
        {
            InitializeComponent();
        }
       
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
            SingleValve Valve = (SingleValve)((Button)e.OriginalSource).Parent;
            if (Valve.IsOpen == true)
            {
                var result = ValveOperationEx.Show("Valve Operation", $"{Valve.ValveID} 밸브를 닫으시겠습니까?");
                switch (result)
                {
                    case ValveOperationExResult.Ok:
                        Valve.IsOpen = !(Valve.IsOpen);
                        MessageBox.Show($"{Valve.ValveID} 밸브 닫음");
                        break;
                    case ValveOperationExResult.Cancel:
                        MessageBox.Show($"{Valve.ValveID} 취소됨1");
                        break;
                }
            }
            else
            {
                var result = ValveOperationEx.Show("Valve Operation", $"{Valve.ValveID} 밸브를 열겠습니까?");
                switch (result)
                {
                    case ValveOperationExResult.Ok:
                        Valve.IsOpen = !(Valve.IsOpen);
                        MessageBox.Show($"{Valve.ValveID} 밸브 열음");
                        break;
                    case ValveOperationExResult.Cancel:
                        MessageBox.Show($"{Valve.ValveID} 취소됨2");
                        break;
                }
            }
        }
    }
}