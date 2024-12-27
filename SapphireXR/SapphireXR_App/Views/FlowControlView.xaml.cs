using SapphireXR_App.Common;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SapphireXR_App.Views
{
    public class NumberBox : TextBox
    {
        public NumberBox() : base()
        {
            PreviewTextInput += OnlyAllowNumber;
        }

        protected void OnlyAllowNumber(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Util.IsTextNumeric(e.Text);
        }
    }

    public class NumberBoxWithMax: TextBox
    {
        public NumberBoxWithMax(): base()
        {
            PreviewTextInput += OnlyAllowNumber;
        }

        protected bool CheckValid(string str)
        {
            if(Util.IsTextNumeric(str) == true)
            {
                string nextValueStr = Text + str;
                int nextValue = int.Parse(nextValueStr);
                if (nextValue <= MaxValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false; ;
            }
        }

        protected void OnlyAllowNumber(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !CheckValid(e.Text);
        }

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set {  SetValue(MaxValueProperty, value); } 
        }

        private static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(int), typeof(NumberBoxWithMax), new PropertyMetadata(int.MinValue));
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
