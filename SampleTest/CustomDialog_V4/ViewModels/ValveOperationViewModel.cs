using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomDialogSample1.Enums;
using System.Windows;

namespace CustomDialogSample1.ViewModels
{
    public partial class ValveOperationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private string _message = string.Empty;
        [ObservableProperty]
        private string _okText = string.Empty;
        [ObservableProperty]
        private string? _cancelText = string.Empty;

        public ValveOperationExResult ValveOperationExResult { get; internal set; } = ValveOperationExResult.Canel;

        [RelayCommand]
        private void Ok(Window window)
        {
            ValveOperationExResult = ValveOperationExResult.Ok;
            window.DialogResult = true;
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            ValveOperationExResult = ValveOperationExResult.Canel;
            window.DialogResult = false;
        }

        public ValveOperationViewModel(string title, string message, string okText, string? cancelText)
        {
            Title = title;
            Message = message;
            OkText = okText;
            CancelText = cancelText;
        }
    }
}
