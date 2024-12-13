using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using SapphireXR_App.Common;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using SapphireXR_App.ViewModels;
using SapphireXR_App.Views;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// FlowController.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FlowController : UserControl
    {
        static FlowController()
        {
            MouseEnterColor = new SolidColorBrush(Color.FromRgb(0x9d, 0xbc, 0xe8));
        }
        public FlowController()
        {
            InitializeComponent();
            ControllerBorderBackground = new SolidColorBrush(Color.FromRgb(0xCC, 0xDF, 0xEF));
        }

        public string ControllerID
        {
            get { return (string)GetValue(ControllerIDProperty); }
            set { SetValue(ControllerIDProperty, value); }
        }

        public static readonly DependencyProperty ControllerIDProperty =
            DependencyProperty.Register("ControllerID", typeof(string), typeof(FlowController), new PropertyMetadata(default));

        public float TargetValue
        {
            get { return (float)GetValue(TargetValueProperty); }
            set { SetValue(TargetValueProperty, value); }
        }

        public static readonly DependencyProperty TargetValueProperty =
            DependencyProperty.Register("TargetValue", typeof(float), typeof(FlowController), new PropertyMetadata(default));

        public float ControlValue
        {
            get { return (float)GetValue(ControlValueProperty); }
            set { SetValue(ControlValueProperty, value); }
        }

        public static readonly DependencyProperty ControlValueProperty =
            DependencyProperty.Register("ControlValue", typeof(float), typeof(FlowController), new PropertyMetadata(default));

        public float CurrentValue
        {
            get { return (float)GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }

        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(float), typeof(FlowController), new PropertyMetadata(default));

        public string buttonBackground
        {
            get { return (string)GetValue(buttonBackgroundProperty); }
            set { SetValue(buttonBackgroundProperty, value); }
        }

        public static readonly DependencyProperty buttonBackgroundProperty =
            DependencyProperty.Register("buttonBackground", typeof(string), typeof(FlowController), new PropertyMetadata(default));


        public bool IsDeviationLimit
        {
            get { return (bool)GetValue(IsDeviationLimitProperty); }
            set { SetValue(IsDeviationLimitProperty, value); }
        }

        public string Type
        {
            get { return (string)GetValue(typeProperty);  }
            set { 
                SetValue(typeProperty, value);
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
            }
        }

        public static readonly DependencyProperty IsDeviationLimitProperty =
            DependencyProperty.Register("IsDeviationLimit", typeof(bool), typeof(FlowController), new PropertyMetadata(default));

        static uint TypeCount = 0;
        public readonly DependencyProperty typeProperty =
            DependencyProperty.Register("TypeProperty" + (TypeCount++), typeof(string), typeof(FlowController), new PropertyMetadata(""));

        static private bool SetMouseEnterColor = false;
        public static SolidColorBrush MouseEnterColor
        {
            get; set;
        }
        public SolidColorBrush ControllerBorderBackground
        {
            get; set;
        }

        private FlowControlView? flowControlView = null;

        private void FlowController_Click(object sender, RoutedEventArgs e) 
        {
            if (flowControlView == null)
            {
                flowController = (FlowController)((Button)e.OriginalSource).Parent;
                if (flowController != null)
                {
                    flowControlView = FlowControllerEx.Show("Flow Controller", $"{flowController.ControllerID} 유량을 변경하시겠습니까?");
                    Point p = TransformToAncestor(Util.FindParent<Window>(this, "mainWindow")).Transform(new Point(0, 0));
                    flowControlView.Left = p.X;
                    flowControlView.Top = p.Y;
                    flowControlView.Topmost = true;
                    ((FlowControlViewModel)(flowControlView.DataContext)).Confirmed += (PopupExResult result, FlowControlViewModel.ControlValues controlValues) => {
                            //여기에서 FlowControlView에서 설정한 속성값들(Targe Value, Current Value ...)을 얻어온다.
                        };
                    ((FlowControlViewModel)(flowControlView.DataContext)).Canceled += (PopupExResult result) => { };
                    flowControlView.Closed += (object? sender, EventArgs e) =>
                    {
                        flowControlView = null; 
                    };
                
                }
            }
            else
            {
                flowControlView.Focus();
            }
        }

        private void flowController_Loaded(object sender, RoutedEventArgs e)
        {
            if (SetMouseEnterColor == false)
            {
                MouseEnterColor = Application.Current.Resources.MergedDictionaries[0]["ValeOnMouseEnterColor"] as SolidColorBrush ?? MouseEnterColor;
                SetMouseEnterColor = true;
            }
        }

        private void controllerBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(sender is Border)
            {
                ((Border)sender).Background = ControllerBorderBackground;
            }
        }

        private void controllerBorder_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Border)
            {
                ((Border)sender).Background = MouseEnterColor;
            }
        }
    }
}
