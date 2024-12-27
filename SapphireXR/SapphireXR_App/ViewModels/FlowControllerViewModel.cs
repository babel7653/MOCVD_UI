using System.ComponentModel;
using System.Windows;
using SapphireXR_App.Controls;
using SapphireXR_App.Views;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using System.Reactive;
using System.Windows.Input;
using SapphireXR_App.Enums;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
using static SapphireXR_App.ViewModels.FlowControlViewModel;

namespace SapphireXR_App.ViewModels
{
    public class FlowControllerViewModel : DependencyObject, INotifyPropertyChanged
    {
        static FlowControllerViewModel()
        {
            MouseEnterColor = Application.Current.Resources.MergedDictionaries[0]["ValveOnMouseEnterColor"] as SolidColorBrush ?? MouseEnterColor;
        }
        public FlowControllerViewModel() 
        {
            OnFlowControllerConfirmedCommand = new RelayCommand<object?>((object? parameter) =>
            {
                object[] parameters = (object[])parameter!;
                PopupExResult result = (PopupExResult)parameters[0];
                FlowControlViewModel.ControlValues controlValues = (FlowControlViewModel.ControlValues)parameters[1];

                if (controlValues.targetValue != null)
                {
                    PLCService.WriteTargetValue(ControllerID, (int)controlValues.targetValue);
                }
                if (controlValues.rampTime != null)
                {
                    PLCService.WriteRampTime(ControllerID, (Int16)controlValues.rampTime);
                }
            });
        }

        internal class ControlValueSubscriber : IObserver<int>
        {
            public ControlValueSubscriber(FlowControllerViewModel viewModel)
            {
                flowControllerViewModel = viewModel;
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
                flowControllerViewModel.ControlValue = value.ToString();
            }

            private FlowControllerViewModel flowControllerViewModel;
        }

        public string ControllerID
        {
            get { return (string)GetValue(ControllerIDProperty); }
            set { SetValue(ControllerIDProperty, value); }
        }

        public static readonly DependencyProperty ControllerIDProperty =
            DependencyProperty.Register("ControllerID", typeof(string), typeof(FlowControllerViewModel), new PropertyMetadata(default));

        public float TargetValue
        {
            get { return (float)GetValue(TargetValueProperty); }
            set { SetValue(TargetValueProperty, value); }
        }

        public static readonly DependencyProperty TargetValueProperty =
            DependencyProperty.Register("TargetValue", typeof(float), typeof(FlowControllerViewModel), new PropertyMetadata(default));

        public string ControlValue
        {
            get { return (string)GetValue(ControlValueProperty); }
            set { 
                SetValue(ControlValueProperty, value);
                OnPropertyChanged(nameof(ControlValue));
            }
        }

        public static readonly DependencyProperty ControlValueProperty =
            DependencyProperty.Register("ControlValue", typeof(string), typeof(FlowControllerViewModel), new PropertyMetadata(default));

        public float CurrentValue
        {
            get { return (float)GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }

        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(float), typeof(FlowControllerViewModel), new PropertyMetadata(default));

        public string buttonBackground
        {
            get { return (string)GetValue(buttonBackgroundProperty); }
            set { SetValue(buttonBackgroundProperty, value); }
        }

        public static readonly DependencyProperty buttonBackgroundProperty =
            DependencyProperty.Register("buttonBackground", typeof(string), typeof(FlowControllerViewModel), new PropertyMetadata(default));


        public bool IsDeviationLimit
        {
            get { return (bool)GetValue(IsDeviationLimitProperty); }
            set { SetValue(IsDeviationLimitProperty, value); }
        }

        public string Type
        {
            get { return (string)GetValue(typeProperty); }
            set
            {
                SetValue(typeProperty, value);
                OnPropertyChanged(nameof(Type));
            }
        }

        public static readonly DependencyProperty IsDeviationLimitProperty =
            DependencyProperty.Register("IsDeviationLimit", typeof(bool), typeof(FlowControllerViewModel), new PropertyMetadata(default));

        static uint TypeCount = 0;
        public readonly DependencyProperty typeProperty =
            DependencyProperty.Register("TypeProperty" + (TypeCount++), typeof(string), typeof(FlowControllerViewModel), new PropertyMetadata(""));

        public static SolidColorBrush MouseEnterColor
        {
            get; set;
        } = new SolidColorBrush(Color.FromRgb(0x9d, 0xbc, 0xe8));
        public SolidColorBrush ControllerBorderBackground
        {
            get; set;
        } = new SolidColorBrush(Color.FromRgb(0xCC, 0xDF, 0xEF));

        public static readonly DependencyProperty BorderBackgroundProperty =
           DependencyProperty.Register("BorderBackground", typeof(SolidColorBrush), typeof(FlowControllerViewModel), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0xCC, 0xDF, 0xEF))));
        public SolidColorBrush BorderBackground
        {
            get { return (SolidColorBrush)GetValue(BorderBackgroundProperty); }
            set
            {
                SetValue(BorderBackgroundProperty, value);
                OnPropertyChanged(nameof(BorderBackground));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand OnLoadedCommand => new RelayCommand<object?>((object? param) =>
        {
            string type = (string)((object[])param!)[0];
            if(type != string.Empty)
            {
                Type = type;
            }
            else
            {
                throw new Exception("Type property of FlowController not set");
            }

            string controllerID = (string)((object[])param)[1];
            if (controllerID != string.Empty)
            {
                ControllerID = controllerID;
            }
            else
            {
                throw new Exception("ControllerID property of FlowController not set");
            }

            switch (Type)
            {
                case "MFC":
                    ControllerBorderBackground = Application.Current.Resources.MergedDictionaries[0]["MFCDisplayColor2"] as SolidColorBrush
                        ?? ControllerBorderBackground;
                    break;

                case "EPC":
                    ControllerBorderBackground = Application.Current.Resources.MergedDictionaries[0]["EPCDisplayColor1"] as SolidColorBrush
                        ?? ControllerBorderBackground;
                    break;
            }
            BorderBackground = ControllerBorderBackground;

            controlValueSubscriber = new ControlValueSubscriber(this);
            ObservableManager<int>.Subscribe("FlowControl." + controllerID + ".ControlValue", controlValueSubscriber);
        });
        public ICommand OnMouseEntered => new RelayCommand(() =>
        {
            BorderBackground = MouseEnterColor;
        });
        public ICommand OnMouseLeaved => new RelayCommand(() =>
        {
            BorderBackground = ControllerBorderBackground;
        });
        public ICommand OnFlowControllerConfirmedCommand;
        public ICommand OnFlowControllerCanceledCommand = new RelayCommand<PopupExResult>((PopupExResult result) =>
        {
        });

        private ControlValueSubscriber? controlValueSubscriber;
    }
}
