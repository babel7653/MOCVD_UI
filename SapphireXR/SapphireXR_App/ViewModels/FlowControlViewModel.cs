using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Enums;
using SapphireXR_App.Views;
using System.ComponentModel;
using System.Windows;

namespace SapphireXR_App.ViewModels
{
    public partial class FlowControlViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _title = string.Empty;
        [ObservableProperty]
        private string _message = string.Empty;
        [ObservableProperty]
        private string _targetValue = string.Empty;

        public PopupExResult PopupExResult { get; internal set; } = PopupExResult.Close;

        [RelayCommand]
        private void Confirm(Window window)
        {
            PopupExResult = PopupExResult.Confirm;
            Confirmed!(PopupExResult.Confirm, new ControlValues { targetValue = double.Parse(TargetValue == null || TargetValue == "" ? "0.0": TargetValue) });
            window.Close();
        }

        [RelayCommand]
        private void Close(Window window)
        {
            PopupExResult = PopupExResult.Close;
            Canceled!(PopupExResult.Close);
            window.Close();
        }
        public FlowControlViewModel(string title, string message)
        {
            Title = title;
            Message = message;
            TargetValue = "";
        }

        public struct ControlValues
        {
            public double targetValue;
        }

        public delegate void ConfiredEventHandler(PopupExResult result, ControlValues controlValues);
        public event ConfiredEventHandler? Confirmed;
        public delegate void CanceledEventHandler(PopupExResult result);
        public event CanceledEventHandler? Canceled;
    }
}
