using SapphireXR_App.Common;
using System.Windows.Controls;
using System.Windows;

namespace SapphireXR_App.Views
{    public class NumberBox : TextBox
    {
        public NumberBox() : base()
        {
            TextChanged += OnlyAllowNumber;
            flowControllerTextBoxValidaterOnlyNumber = new FlowControllerTextBoxValidaterOnlyNumber();
        }

        protected void OnlyAllowNumber(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox != null)
            {
                string validatedFlowControllerValue = flowControllerTextBoxValidaterOnlyNumber.valdiate(textBox);
                if (validatedFlowControllerValue != textBox.Text)
                {
                    int caretIndex = textBox.CaretIndex - 1;
                    textBox.Text = validatedFlowControllerValue;
                    textBox.CaretIndex = caretIndex;
                }
                
            }
        }

        private FlowControllerTextBoxValidaterOnlyNumber flowControllerTextBoxValidaterOnlyNumber;
    }

    public class NumberBoxWithMax : TextBox
    {
        public NumberBoxWithMax() : base()
        {
            flowControllerTextBoxValidater = new FlowControllerTextBoxValidaterMaxValue();
            TextChanged += onlyAllowNumberWithMax;
        }

        protected void onlyAllowNumberWithMax(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox != null)
            {
                (string validatedFlowControllerValue, FlowControllerTextBoxValidaterMaxValue.Result result) = flowControllerTextBoxValidater.valdiate(textBox, (uint)MaxValue);
                switch(result)
                {
                    case FlowControllerTextBoxValidaterMaxValue.Result.Valid:
                        textBox.ClearValue(ToolTipProperty);
                        break;

                    case FlowControllerTextBoxValidaterMaxValue.Result.NotNumber:
                    case FlowControllerTextBoxValidaterMaxValue.Result.ExceedMax:
                        int caretIndex = textBox.CaretIndex - 1;
                        textBox.Text = validatedFlowControllerValue;
                        textBox.CaretIndex = caretIndex;
                        if(result == FlowControllerTextBoxValidaterMaxValue.Result.ExceedMax)
                        {
                            ToolTip toolTip = new ToolTip();
                            toolTip.Content = "최대값 " + MaxValue + "보다 클 수 없습니다.";
                            textBox.ToolTip = toolTip;
                        }
                        break;
                }
            }
        }

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        private static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(int), typeof(NumberBoxWithMax), new PropertyMetadata(int.MinValue));
        private FlowControllerTextBoxValidaterMaxValue flowControllerTextBoxValidater;
    }
}
