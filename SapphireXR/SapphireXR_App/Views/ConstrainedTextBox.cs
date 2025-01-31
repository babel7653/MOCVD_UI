using SapphireXR_App.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SapphireXR_App.Views
{    public class NumberBox : TextBox
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

    public class NumberBoxWithMax : TextBox
    {
        public NumberBoxWithMax() : base()
        {
            PreviewTextInput += OnlyAllowNumber;
        }

        protected bool CheckValid(string str)
        {
            if (Util.IsTextNumeric(str) == true)
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
            TextBox? textbox = sender as TextBox;
            if (textbox != null)
            {
                Util.OnlyAllowConstrainedNumber(e, textbox.Text, e.Text, MaxValue);
            }
        }

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        private static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(int), typeof(NumberBoxWithMax), new PropertyMetadata(int.MinValue));
    }
}
