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

namespace SapphireXR_App.ViewModels.FlowController
{
    public abstract class FlowControllerViewModelBase : DependencyObject, INotifyPropertyChanged
    {
        static FlowControllerViewModelBase()
        {
            MouseEnterColor = Application.Current.Resources.MergedDictionaries[0]["ValveOnMouseEnterColor"] as SolidColorBrush ?? MouseEnterColor;
        }

        public string ControllerID
        {
            get { return (string)GetValue(ControllerIDProperty); }
            set { SetValue(ControllerIDProperty, value); }
        }

        public static readonly DependencyProperty ControllerIDProperty =
            DependencyProperty.Register("ControllerID", typeof(string), typeof(FlowControllerViewModelBase), new PropertyMetadata(default));

        public string ControlValue
        {
            get { return (string)GetValue(ControlValueProperty); }
            set { 
                SetValue(ControlValueProperty, value);
                OnPropertyChanged(nameof(ControlValue));
            }
        }

        public static readonly DependencyProperty ControlValueProperty =
            DependencyProperty.Register("ControlValue", typeof(string), typeof(FlowControllerViewModelBase), new PropertyMetadata(default));

        public string buttonBackground
        {
            get { return (string)GetValue(buttonBackgroundProperty); }
            set { SetValue(buttonBackgroundProperty, value); }
        }

        public static readonly DependencyProperty buttonBackgroundProperty =
            DependencyProperty.Register("buttonBackground", typeof(string), typeof(FlowControllerViewModelBase), new PropertyMetadata(default));

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
            DependencyProperty.Register("IsDeviationLimit", typeof(bool), typeof(FlowControllerViewModelBase), new PropertyMetadata(default));

        static uint TypeCount = 0;
        public readonly DependencyProperty typeProperty =
            DependencyProperty.Register("TypeProperty" + (TypeCount++), typeof(string), typeof(FlowControllerViewModelBase), new PropertyMetadata(""));

        public static SolidColorBrush MouseEnterColor
        {
            get; set;
        } = new SolidColorBrush(Color.FromRgb(0x9d, 0xbc, 0xe8));
        public SolidColorBrush ControllerBorderBackground
        {
            get; set;
        } = new SolidColorBrush(Color.FromRgb(0xCC, 0xDF, 0xEF));

        public static readonly DependencyProperty BorderBackgroundProperty =
           DependencyProperty.Register("BorderBackground", typeof(SolidColorBrush), typeof(FlowControllerViewModelBase), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0xCC, 0xDF, 0xEF))));
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

        protected virtual void onLoaded(object? param)
        {
            string type = (string)((object[])param!)[0];
            if (type == string.Empty)
            {
                throw new Exception("Type property of FlowController not set");
            }

            string controllerID = (string)((object[])param)[1];
            if (controllerID == string.Empty)
            {
                throw new Exception("ControllerID property of FlowController not set");
            }

            onLoaded(type, controllerID);
        }

        protected virtual void onLoaded(string type, string controllerID)
        {
            Type = type;
            ControllerID = controllerID;
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

                case "Reactor":
                    ControllerBorderBackground = Application.Current.Resources.MergedDictionaries[0]["ReactorDisplayColor1"] as SolidColorBrush
                       ?? ControllerBorderBackground;
                    break;
            }
            BorderBackground = ControllerBorderBackground;
        }

        public ICommand OnLoadedCommand => new RelayCommand<object?>(onLoaded);

        protected abstract void onClicked(object[]? args);

        public ICommand OnMouseEntered => new RelayCommand(() =>
        {
            BorderBackground = MouseEnterColor;
        });
        public ICommand OnMouseLeaved => new RelayCommand(() =>
        {
            BorderBackground = ControllerBorderBackground;
        });

        public ICommand OnClickedCommand => new RelayCommand<object[]?>(onClicked);
    }
}
