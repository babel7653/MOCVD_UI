using SapphireXR_App.Controls;
using SapphireXR_App.Models;
using SapphireXR_App.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;
using SapphireXR_App.Enums;
using System.Windows;



namespace SapphireXR_App.Views
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(HomeViewModel));
        }

        private void BlockValve_Click(object sender, MouseButtonEventArgs e)
        {
            BlockValve blockValve = (BlockValve)sender;
            if (blockValve.IsOpen == true)
            {
                var result = ValveOperationEx.Show("Valve Operation", $"{blockValve.ValveID} 밸브를 질소가스로 변경하시겠습니까?");
                switch (result)
                {
                    case ValveOperationExResult.Ok:
                        blockValve.IsOpen = false;
                        MessageBox.Show($"{blockValve.ValveID} 밸브 닫음");
                        break;
                    case ValveOperationExResult.Cancel:
                        MessageBox.Show($"{blockValve.ValveID} 취소됨");
                        break;
                }
            }
            else
            {
                var result = ValveOperationEx.Show("Valve Operation", $"{blockValve.ValveID} 밸브를 공정가스로 변경하시겠습니까?");
                switch (result)
                {
                    case ValveOperationExResult.Ok:
                        blockValve.IsOpen = true;
                        MessageBox.Show($"{blockValve.ValveID} 밸브 열음");
                        break;
                    case ValveOperationExResult.Cancel:
                        MessageBox.Show($"{blockValve.ValveID} 취소됨");
                        break;
                }
            }
        }
    }
}
