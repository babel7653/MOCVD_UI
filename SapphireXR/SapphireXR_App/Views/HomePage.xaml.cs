using SapphireXR_App.Controls;
using SapphireXR_App.Models;
using SapphireXR_App.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;
using SapphireXR_App.Enums;
using System.Windows;
using TwinCAT.TypeSystem;



namespace SapphireXR_App.Views
{
    public partial class HomePage : Page
    {
        public static bool IsMaintenanceMode { get; set; } = false;
        public HomePage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(HomeViewModel));
        }

        private void SwitchingValve_Click(object sender, MouseButtonEventArgs e)
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

        private void BypassValve_Click(object sender, MouseButtonEventArgs e)
        {
            SwitchingValve Valve = (SwitchingValve)sender;
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

        private void SingleValve_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void ButterflyValve_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void LeakTestValve_Click(object sender, MouseButtonEventArgs e)
        {
            LeakTestValve Valve = (LeakTestValve)((Button)e.OriginalSource).Parent;
            //LeakTestValve Valve = (LeakTestValve)sender;
            //LeakTestValve Valve = e.Source as LeakTestValve;

            if (Valve.IsOpen == true)
            {
                var result = ValveOperationEx.Show("Valve Operation", $"Leak Test 밸브 {Valve.ValveID}를 닫으시겠습니까?");
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
                var result = ValveOperationEx.Show("Valve Operation", $"Leak Test 밸브{Valve.ValveID} 를 열겠습니까?");
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
