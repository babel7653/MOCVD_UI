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
using SapphireXR_App.Enums;
using SapphireXR_App.Models;

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

        private void SwitchingValve_Click(object sender, RoutedEventArgs e)
        {
            SwitchingValve Valve = (SwitchingValve)((Button)e.OriginalSource).Parent;
            if (Valve.IsOpen == true)
            {
                var result = ValveOperationEx.Show("Valve Operation", $"{Valve.ValveID} 밸브를 질소가스로 변경하시겠습니까?");
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
                var result = ValveOperationEx.Show("Valve Operation", $"{Valve.ValveID} 밸브를 공정가스로 변경하시겠습니까?");
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
