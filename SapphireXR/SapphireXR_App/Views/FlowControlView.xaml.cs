using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SapphireXR_App.Views
{
    public class NumberBox: TextBox
    {
        public NumberBox(): base()
        {
            PreviewTextInput += OnlyAllowNumber;
        }

        protected static bool IsTextNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9]");
            return reg.IsMatch(str);
        }

        protected void OnlyAllowNumber(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
        }
    }
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
    }
}
