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

        public PopupExResult PopupExResult { get; internal set; } = PopupExResult.Close;

        [RelayCommand]
        private void Confirm(Window window)
        {
            //PopupExResult = PopupExResult.Confirm;
            //window.DialogResult = true;
            // TO DO - 확인시 Target Value, RampTime 변경
        }

        [RelayCommand]
        private void Close(Window window)
        {
            PopupExResult = PopupExResult.Close;
            window.DialogResult = false;
        }
        public FlowControlViewModel(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }
}
