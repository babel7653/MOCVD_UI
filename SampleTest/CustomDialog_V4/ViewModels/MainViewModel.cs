using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace CustomDialogSample1.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public IRelayCommand ValveOperationCommand { get; set; }
        public IRelayCommand ValveOperationCommand1 { get; set; }
        public IRelayCommand ValveOperationChkCommand { get; set; }
        public IRelayCommand ValveOperationUnChkCommand { get; set; }


        public MainViewModel()
        {
            ValveOperationCommand = new RelayCommand(ValveOperation);
            ValveOperationCommand1 = new RelayCommand<string>(ValveOperation1);
            ValveOperationChkCommand = new RelayCommand<ToggleButton>(ValveOperationChk);
            ValveOperationUnChkCommand = new RelayCommand<ToggleButton>(ValveOperationUnChk);


        }

        private void ValveOperationUnChk(ToggleButton? button)
        {
            //ToggleButton toggleButton = button as ToggleButton;
            // button.IsChecked = !button.IsChecked;
            //toggleButton.IsChecked = false;
            MessageBox.Show("UnChked", "Valve Operation");
        }

        private void ValveOperationChk(ToggleButton? button)
        {
            MessageBox.Show("Chked", "Valve Operation");
        }

        private void ValveOperation()
        {
            MessageBox.Show("밸브를 질소로 변경하시겠습니까?", "Valve Operation");
        }
        private void ValveOperation1(string ValveNo)
        {
            MessageBox.Show($"밸브 {ValveNo}를 질소로 변경하시겠습니까?", "Valve Operation");
        }

    }
}
