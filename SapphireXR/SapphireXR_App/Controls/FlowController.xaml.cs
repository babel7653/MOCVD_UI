using System.Windows;
using System.Windows.Controls;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// FlowController.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FlowController : UserControl
    {
        public FlowController()
        {
            InitializeComponent();
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

        private void ControllerButton_Loaded(object sender, RoutedEventArgs e)
        {
            FlowController display = (FlowController)sender;
        }

        public string Type
        {
            get { return (string)GetValue(typeProperty);  }
            set { SetValue(typeProperty, value); }
        }

        static uint TypeCount = 0;
        public readonly DependencyProperty typeProperty =
            DependencyProperty.Register("TypeProperty" + (TypeCount++), typeof(string), typeof(FlowController), new PropertyMetadata(""));

    }
}
