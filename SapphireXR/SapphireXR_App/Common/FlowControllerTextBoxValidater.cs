using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Models;
using System.ComponentModel;
using System.Windows.Controls;

namespace SapphireXR_App.Common
{
    internal abstract class FlowControllerTextBoxValidater
    {
        protected string prevText = "";
      
    }
    internal class FlowControllerTextBoxValidaterOnlyNumber: FlowControllerTextBoxValidater
    {
        public string valdiate(TextBox textBox)
        {
            if (textBox.Text == "" || Util.IsTextNumeric(textBox.Text) == true)
            {
                prevText = textBox.Text;
            }

            return prevText;
        }
    }
    internal class FlowControllerTextBoxValidaterMaxValue: FlowControllerTextBoxValidater
    {
        public enum Result { Valid = 0, NotNumber = 1, ExceedMax, Undefined };

        public (string, Result) valdiate(TextBox textBox, uint maxValue)
        {
            if(textBox.Text == "")
            {
                prevText = textBox.Text;
                return (prevText, Result.Valid);
            }

            if(Util.IsTextNumeric(textBox.Text) == false)
            {
                return (prevText, Result.NotNumber);
            }

            if(maxValue < uint.Parse(textBox.Text))
            {
                return (prevText, Result.ExceedMax);
            }

            prevText = textBox.Text;
            return (prevText, Result.Valid);
        }

        public (string, Result) valdiate(TextBox textBox, string flowControllerID)
        {
            return valdiate(textBox, (uint)PLCService.ReadMaxValue(flowControllerID));
        }
    }
    
    internal abstract class FlowControllerDataGridTextColumnTextBoxValidater
    {
        internal FlowControllerDataGridTextColumnTextBoxValidater(ObservableObject viewModel, string recipesPropertyName)
        {
            viewModel.PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
            {
                if (args.PropertyName == recipesPropertyName)
                {
                    prevTexts.Clear();
                }
            };
        }

        protected abstract FlowControllerTextBoxValidater CreateValidater();

        protected FlowControllerTextBoxValidater get(TextBox textBox)
        {
            FlowControllerTextBoxValidater? validater = null;
            if (prevTexts.TryGetValue(textBox, out validater) == false)
            {
                validater = CreateValidater();
                prevTexts.Add(textBox, validater);
            }
            return validater;
        }

        protected Dictionary<TextBox, FlowControllerTextBoxValidater> prevTexts = new Dictionary<TextBox, FlowControllerTextBoxValidater>();
    }
    internal class FlowControllerDataGridTextColumnTextBoxValidaterOnlyNumber : FlowControllerDataGridTextColumnTextBoxValidater
    {
        internal FlowControllerDataGridTextColumnTextBoxValidaterOnlyNumber(ObservableObject viewModel, string recipesPropertyName) : base(viewModel, recipesPropertyName) { }

        protected override FlowControllerTextBoxValidater CreateValidater()
        {
            return new FlowControllerTextBoxValidaterOnlyNumber();
        }

        public string validate(TextBox textBox)
        {
            return ((FlowControllerTextBoxValidaterOnlyNumber)get(textBox)).valdiate(textBox);
        }
    }
    internal class FlowControllerDataGridTextColumnTextBoxValidaterMaxValue: FlowControllerDataGridTextColumnTextBoxValidater
    {
        internal FlowControllerDataGridTextColumnTextBoxValidaterMaxValue(ObservableObject viewModel, string recipesPropertyName): base(viewModel, recipesPropertyName) { }

        protected override FlowControllerTextBoxValidater CreateValidater()
        {
            return new FlowControllerTextBoxValidaterMaxValue();
        }

        public (string, FlowControllerTextBoxValidaterMaxValue.Result) validate(TextBox textBox, string flowControllerID)
        {
            return ((FlowControllerTextBoxValidaterMaxValue)get(textBox)).valdiate(textBox, flowControllerID);
        }

        public (string, FlowControllerTextBoxValidaterMaxValue.Result) validate(TextBox textBox, uint maxValue)
        {
            return ((FlowControllerTextBoxValidaterMaxValue)get(textBox)).valdiate(textBox, maxValue);
        }

        public (string?, FlowControllerTextBoxValidaterMaxValue.Result) validate(TextBox textBox, TextChangedEventArgs e)
        {
            DataGridCell? dataGridCell = textBox.Parent as DataGridCell;
            if (dataGridCell != null)
            {
                string? flowControlField = dataGridCell.Column.Header as string;
                if (flowControlField != null)
                {
                    string? flowControllerID = null;
                    if (Util.RecipeFlowControlFieldToControllerID.TryGetValue(flowControlField, out flowControllerID) == true)
                    {
                        return validate(textBox, flowControllerID);
                    }
                }
            }
            return (null, FlowControllerTextBoxValidaterMaxValue.Result.Undefined);
        }
    }
}
