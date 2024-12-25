using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using SapphireXR_App.Views;
using System.ComponentModel;
using System.Reactive;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls.Ribbon.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using static SapphireXR_App.ViewModels.FlowControlViewModel;

namespace SapphireXR_App.ViewModels
{
    public partial class FlowControlViewModel : ObservableObject, IObserver<float>
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
        [ObservableProperty]
        private SolidColorBrush _fontColor = new SolidColorBrush(Colors.Black);

        public PopupExResult PopupExResult { get; internal set; } = PopupExResult.Close;

        internal class ControlValueSubscriber : IObserver<int>
        {
            public ControlValueSubscriber(FlowControlViewModel viewModel)
            {
                flowControlViewModel = viewModel;
            }
            void IObserver<int>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<int>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<int>.OnNext(int value)
            {
                flowControlViewModel.ControlValue = value.ToString();
            }

            private FlowControlViewModel flowControlViewModel;
        }

        internal class CurrwentValueSubscriber : IObserver<int>
        {
            public CurrwentValueSubscriber(FlowControlViewModel viewModel)
            {
                flowControlViewModel = viewModel;
            }
            void IObserver<int>.OnCompleted()
            {
                throw new NotImplementedException();
            }

            void IObserver<int>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<int>.OnNext(int value)
            {
                flowControlViewModel.CurrentValue = value.ToString();
            }

            private FlowControlViewModel flowControlViewModel;
        }

        [RelayCommand]
        private void Confirm(Window window)
        {
            PopupExResult = PopupExResult.Confirm;
            Confirmed!(PopupExResult.Confirm, new ControlValues { targetValue = uint.Parse(TargetValue), rampTime = uint.Parse(RampTime), controlValue = uint.Parse(ControlValue), 
                currentValue = uint.Parse(CurrentValue), deviation = uint.Parse(Deviation), maxValue = uint.Parse(MaxValue) });
            dispose();
            window.Close();
        }

        [RelayCommand]
        private void Close(Window window)
        {
            PopupExResult = PopupExResult.Close;
            Canceled!(PopupExResult.Close);
            dispose();
            window.Close();
        }

        void dispose()
        {
            controlValueSubscriberDisposable.Dispose();
            currentValueSubscriberDisposable.Dispose();
        }

        void IObserver<float>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<float>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<float>.OnNext(float value)
        {
            throw new NotImplementedException();
        }

        public FlowControlViewModel(string title, string message, string fcID)
        {
            Title = title;
            Message = message;
            TargetValue = string.Empty;
            RampTime = string.Empty;
            Deviation = string.Empty;
            CurrentValue = string.Empty;
            ControlValue = string.Empty;
            MaxValue = PLCService.ReadMaxValue(fcID).ToString();
            FontColor = OnNormal;
            PropertyChanged += (object? sender, PropertyChangedEventArgs e) =>
            {
                if(e.PropertyName == "CurrentValue" || e.PropertyName == "ControlValue")
                {
                    if (Util.IsTextNumeric(CurrentValue) && Util.IsTextNumeric(ControlValue) && Util.IsTextNumeric(MaxValue))
                    {
                        Deviation = ((int)(((double)(Math.Abs(int.Parse(CurrentValue) - int.Parse(ControlValue))) / double.Parse(MaxValue)) * 100.0)).ToString();
                    }
                } 
            };
            controlValueSubscriber = new ControlValueSubscriber(this);
            currwentValueSubscriber = new CurrwentValueSubscriber(this);
            currentValueSubscriberDisposable = ObservableManager<int>.Subscribe("FlowControl." + fcID + ".CurrentValue", currwentValueSubscriber);
            controlValueSubscriberDisposable = ObservableManager<int>.Subscribe("FlowControl." + fcID + ".ControlValue", controlValueSubscriber);
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


        private static readonly SolidColorBrush OnWrongTextFormat = new SolidColorBrush(Colors.Red);
        private static readonly SolidColorBrush OnNormal = new SolidColorBrush(Colors.Red);

        private ControlValueSubscriber controlValueSubscriber;
        private CurrwentValueSubscriber currwentValueSubscriber;
        private IDisposable currentValueSubscriberDisposable;
        private IDisposable controlValueSubscriberDisposable;
    }
}
