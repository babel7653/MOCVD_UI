using CommunityToolkit.Mvvm.Input;
using CustomDialogSample1.Enums;
using CustomDialogSample1.Models;
using CustomDialogSample1.ViewModels;
using CustomDialogSample1.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace CustomDialogSample1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(MainViewModel));
        }

        private void btnOkCancel_Click(object sender, RoutedEventArgs e)
        {
            var okCancelDialog = new OkCancelDialog(title: "This is the Title", message: "Test Message");
            bool? result = okCancelDialog.ShowDialog();
            MessageBox.Show(result.ToString());
        }

        private void btnMessageBox_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBoxEx.Show("종료", "입력중인 데이터가 있습니다.\n어떻게 할까요?", "저장 후 종료", "취소");
            switch (result)
            {
                case MessageBoxExResult.Button1:
                    MessageBox.Show("저장 후 종료 선택됨");
                    break;
                case MessageBoxExResult.Button2:
                    MessageBox.Show("저장 안하고 종료 선택됨");
                    break;
                case MessageBoxExResult.Button3:
                    MessageBox.Show("취소 선택됨");
                    break;
            }
        }
        private void btnInputBox_Click(object sender, RoutedEventArgs e)
        {
            string? result = InputBoxEx.Show("아무말 메세지", "아무말이나 적어보세요 ^^");


            if (result == null)
            {
                MessageBox.Show("취소됨");
            }
            else
            {
                MessageBox.Show(result);
            }
        }

        private void btnValveOperation_Click(object sender, RoutedEventArgs e)
        {
            var result = ValveOperationEx.Show("Valve Operation", "밸브를 여시겠습니까?");
            switch (result)
            {
                case ValveOperationExResult.Ok:
                    MessageBox.Show("밸브 열음");
                    break;
                case ValveOperationExResult.Canel:

                    MessageBox.Show("취소됨");
                    break;
                    //TODO : 밸브 열림/닫힘 상태 확인 후 메세지 뛰우고 닫혔을 때 -> 열고, 열렸을 때 -> 담음
            }
        }

        private void btnFlowControl_Click(object sender, RoutedEventArgs e)
        {
            string? result = FlowControlEx.Show("Flow Controller", "유량 제어 설정값 변경");

            if (result == null)
            {
                MessageBox.Show("취소됨");
            }
            else
            {
                MessageBox.Show(result);
            }

        }

        private void V01_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            //MessageBoxResult result = MessageBox.Show($"질소가스로 변경하시겠습니까?", $"밸브", MessageBoxButton.OKCancel);
            var result = ValveOperationEx.Show("Valve Operation", "질소가스로 변경하시겠습니까?");
            switch (result)
            {
                case ValveOperationExResult.Ok:
                    MessageBox.Show("밸브 열음");
                    break;
                case ValveOperationExResult.Canel:
                    MessageBox.Show("취소됨");
                    toggleButton.IsChecked = false;
                    break;
                    //break;
                    //TODO : 밸브 열림/닫힘 상태 확인 후 메세지 뛰우고 닫혔을 때 -> 열고, 열렸을 때 -> 담음
            }
        }
        private void V01_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            //MessageBoxResult result = MessageBox.Show($"공정가스로 변경하시겠습니까?", $"밸브", MessageBoxButton.OKCancel);
            var result = ValveOperationEx.Show("Valve Operation", "질소가스로 변경하시겠습니까?");
            switch (result)
            {
                case ValveOperationExResult.Ok:
                    MessageBox.Show("밸브 닫음");
                    break;
                case ValveOperationExResult.Canel:

                    MessageBox.Show("취소됨");
                    toggleButton.IsChecked = true;
                    { return; }
                    //break;
                    //TODO : 밸브 열림/닫힘 상태 확인 후 메세지 뛰우고 닫혔을 때 -> 열고, 열렸을 때 -> 담음
            }
        }

        private void V04_Clicked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            if (toggleButton.IsChecked == true)
            {
                var result = ValveOperationEx.Show("Valve Operation", "질소가스로 변경하시겠습니까?");
                switch (result)
                {
                    case ValveOperationExResult.Ok:
                        MessageBox.Show("질소가스 변경");
                        break;
                    case ValveOperationExResult.Canel:
                        toggleButton.IsChecked = false;
                        MessageBox.Show("취소됨");
                        break;
                }
            }
            else
            {
                var result = ValveOperationEx.Show("Valve Operation", "공정가스로 변경하시겠습니까?");
                switch (result)
                {
                    case ValveOperationExResult.Ok:
                        MessageBox.Show("공정가스 변경");
                        break;
                    case ValveOperationExResult.Canel:
                        toggleButton.IsChecked = true;
                        MessageBox.Show("취소됨");
                        break;
                }
            }


        }

        private void V06_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.IsEnabled == true)
            {
                var result = ValveOperationEx.Show("Valve Operation", $"{button.IsEnabled} 질소 가스로 변경하시겠습니까?");
                switch (result)
                {
                    case ValveOperationExResult.Ok:
                        MessageBox.Show($"{button.IsEnabled} 질소가스 변경");
                        button.IsEnabled = false;
                        break;
                    case ValveOperationExResult.Canel:
                        MessageBox.Show($"{button.IsEnabled} 취소됨");
                        break;
                }
            }
            else
            {
                var result = ValveOperationEx.Show("Valve Operation", $"{button.IsEnabled} 수소 가스로 변경하시겠습니까?");
                switch (result)
                {
                    case ValveOperationExResult.Ok:
                        MessageBox.Show($"{button.IsEnabled }질소가스 변경");
                        button.IsEnabled = true;
                        break;
                    case ValveOperationExResult.Canel:
                        MessageBox.Show($"{button.IsEnabled} 취소됨");
                        break;
                }
            }
        }
    }
}