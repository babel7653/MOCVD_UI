using System.Windows;
using System.Windows.Controls;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;

namespace SapphireXR_App.Controls
{
    public partial class BypassValve : Valve
    {
        public BypassValve()
        {
            InitializeComponent();
        }
        private void BypassValve_Click(object sender, RoutedEventArgs e)
        {
            BypassValve Valve = (BypassValve)((Button)e.OriginalSource).FindName("bypassValve");
            if (Valve.IsOpen == true)
            {
                var result = ValveOperationEx.Show("Valve Operation", $"{Valve.ValveID} 밸브를 Bypass로 변경하시겠습니까?");
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
                var result = ValveOperationEx.Show("Valve Operation", $"{Valve.ValveID} 밸브를 Source 공급으로 변경하시겠습니까?");
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
