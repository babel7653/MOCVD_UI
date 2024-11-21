using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CustomDialogSample1.Enums;
using System.Windows;

namespace CustomDialogSample1.ViewModels
{
    public partial class MessageBoxViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private string _message = string.Empty;
        [ObservableProperty]
        private string _okText = string.Empty;
        [ObservableProperty]
        private string? _noText;
        [ObservableProperty]
        private string? _cancelText;

        [ObservableProperty]
        private Visibility _noVisibilty;
        [ObservableProperty]
        private Visibility _cancelVisibilty;

        public MessageBoxExResult MessageBoxExResult { get; internal set; } = MessageBoxExResult.Cancel;

        [RelayCommand]
        private void Ok(Window window)
        {
            MessageBoxExResult = MessageBoxExResult.Ok;
            window.DialogResult = true;
        }

        [RelayCommand]
        private void No(Window window)
        {
            MessageBoxExResult = MessageBoxExResult.No;
            window.DialogResult = false;
        }

        public MessageBoxViewModel(string title, string message, string okText, string? noText, string? cancelText)
        {
            Title = title;
            Message = message;
            OkText = okText;
            NoText = noText;
            CancelText = cancelText;

            NoVisibilty = string.IsNullOrEmpty(noText) ? Visibility.Collapsed : Visibility.Visible;
            CancelVisibilty = string.IsNullOrEmpty(cancelText) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
