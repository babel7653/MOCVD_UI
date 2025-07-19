using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SapphireXR_App.Views
{
    public partial class FlowControlView : Window
    {
        private void MessageBoxView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        public FlowControlView()
        {
            InitializeComponent();
            MouseLeftButtonDown += MessageBoxView_MouseLeftButtonDown;
        }

        public bool IsConfirmButtonEnabled
        {
            get { return (bool)GetValue(IsConfirmButtonEnabledProperty);  }
            set {  SetValue(IsConfirmButtonEnabledProperty, value); }
        }
        private static readonly DependencyProperty IsConfirmButtonEnabledProperty = DependencyProperty.Register("IsConfirmButtonEnabled", typeof(bool), typeof(FlowControlView), new PropertyMetadata(false));

        private TextBox? targetValue;
        private TextBox? rampTime;
        private void FlowControlDialog_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = (e.Source as TextBox);
            if(textBox != null)
            {
                switch(textBox.Name)
                {
                    case "txtInput":
                        targetValue ??= textBox;
                        IsConfirmButtonEnabled = !string.IsNullOrEmpty(rampTime?.Text) && !string.IsNullOrEmpty(targetValue.Text);
                        break;

                    case "txtInput1":
                        rampTime ??= textBox;
                        IsConfirmButtonEnabled = !string.IsNullOrEmpty(targetValue?.Text) && !string.IsNullOrEmpty(rampTime.Text) && 0 < int.Parse(rampTime.Text);
                        break;
                }
            }
        }
    }
}
