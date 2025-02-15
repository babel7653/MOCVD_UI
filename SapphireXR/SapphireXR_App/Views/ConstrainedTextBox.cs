﻿using SapphireXR_App.Common;
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
            flowControllerTextBoxValidater = new FlowControllerTextBoxValidater();
            PreviewTextInput += onlyAllowNumber;
            TextChanged += validateWithinMax;
        }

        protected void onlyAllowNumber(object sender, TextCompositionEventArgs e)
        {
            Util.OnlyAllowNumber(e, e.Text);
        }

        protected void validateWithinMax(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.Text = flowControllerTextBoxValidater.valdiate(textBox, (uint)MaxValue);
            }
        }

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        private static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(int), typeof(NumberBoxWithMax), new PropertyMetadata(int.MinValue));
        private FlowControllerTextBoxValidater flowControllerTextBoxValidater;
    }
}
