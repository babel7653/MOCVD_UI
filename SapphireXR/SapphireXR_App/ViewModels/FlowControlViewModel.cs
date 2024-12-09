using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Enums;
using System.Windows;

namespace SapphireXR_App.ViewModels
{
    public partial class FlowControlViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private string _message = string.Empty;

        public PopupExResult PopupExResult { get; internal set; } = PopupExResult.Cancel;

        [RelayCommand]
        private void Ok(Window window)
        {
            PopupExResult = PopupExResult.Ok;
            window.DialogResult = true;
        }

        [RelayCommand]
        private void Cancel(Window window)
        {
            PopupExResult = PopupExResult.Cancel;
            window.DialogResult = false;
        }
        public FlowControlViewModel(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }
}
