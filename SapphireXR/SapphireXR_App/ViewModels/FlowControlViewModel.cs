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
        [ObservableProperty]
        private string _rampTime = string.Empty;
        [ObservableProperty]
        private string _deviation = string.Empty;
        [ObservableProperty]
        private string _currentValue = string.Empty;
        [ObservableProperty]
        private string _controlValue = string.Empty;
        [ObservableProperty]
        private string _maxValue = string.Empty;

        public PopupExResult PopupExResult { get; internal set; } = PopupExResult.Close;

        

        [RelayCommand]
        private void Confirm(Window window)
        {
            PopupExResult = PopupExResult.Confirm;
            Confirmed!(PopupExResult.Confirm, new ControlValues { targetValue = uint.Parse(TargetValue), rampTime = uint.Parse(RampTime), controlValue = uint.Parse(ControlValue), 
                currentValue = uint.Parse(CurrentValue), deviation = uint.Parse(Deviation), maxValue = uint.Parse(MaxValue) });
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
            TargetValue = string.Empty;
            RampTime = string.Empty;
            Deviation = string.Empty;
            CurrentValue = string.Empty;
            ControlValue = string.Empty;
            MaxValue = string.Empty;
        }

        public struct ControlValues
        {
            public uint targetValue;
            public uint rampTime;
            public uint deviation;
            public uint currentValue;
            public uint controlValue;
            public uint maxValue;
        }

        public delegate void ConfiredEventHandler(PopupExResult result, ControlValues controlValues);
        public event ConfiredEventHandler? Confirmed;
        public delegate void CanceledEventHandler(PopupExResult result);
        public event CanceledEventHandler? Canceled;
    }
}
