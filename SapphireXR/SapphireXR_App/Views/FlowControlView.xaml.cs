using SapphireXR_App.Common;
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

        protected bool IsTextNumeric(string str, int maxValue)
        {
            if(Util.IsTextNumeric(str) == true)
            {
                string nextValueStr = Text + str;
                int nextValue = int.Parse(nextValueStr);
                if (nextValue <= maxValue)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            else
            {
                return true;
            }
        }

        protected void OnlyAllowNumber(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text, int.Parse(MaxValue));
        }

        public string MaxValue
        {
            get { return (string)GetValue(MaxValueProperty); }
            set {  
                if(Util.IsTextNumeric(value) == true)
                {
                    SetValue(MaxValueProperty, value);
                    maxValue = int.Parse(value);
                }
                else
                {
                    throw new FormatException("MaxValue for FlowControlView is not valid: to set is " + value);
                }
            } 
        }

        private static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(string), typeof(NumberBox), new PropertyMetadata(""));
        private int maxValue = int.MinValue;
       
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
