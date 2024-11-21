using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;

namespace CustomDialogSample1.ViewModels
{
    public partial class FlowControlViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private string _prompt = string.Empty;
        [ObservableProperty]
        private string _response = string.Empty;

        [RelayCommand]
        private void Ok(Window window)
        {
            window.DialogResult = true;
        }

        public FlowControlViewModel(string title, string prompt, string defaultInputMessage)
        {
            Title = title;
            Prompt = prompt;
            Response = defaultInputMessage;
        }
    }
}
