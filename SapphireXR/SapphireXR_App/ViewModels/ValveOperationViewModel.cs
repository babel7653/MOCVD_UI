using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Enums;
using System.Windows;

namespace SapphireXR_App.ViewModels
{
    public partial class ValveOperationViewModel : ObservableObject

    {
        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private string _message = string.Empty;

        public ValveOperationExResult ValveOperationExResult { get; internal set; } = ValveOperationExResult.Cancel;

        [RelayCommand]
        private void Ok(Window window)
        {
            ValveOperationExResult = ValveOperationExResult.Ok;
            window.DialogResult = true;
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            ValveOperationExResult = ValveOperationExResult.Cancel;
            window.DialogResult = false;
        }
        public ValveOperationViewModel(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }
}
