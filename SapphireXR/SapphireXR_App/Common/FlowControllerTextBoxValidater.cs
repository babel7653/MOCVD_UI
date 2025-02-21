using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SapphireXR_App.Common
{
    internal class FlowControllerTextBoxValidater
    {
        public string valdiate(TextBox textBox, uint maxValue)
        {
            if (textBox.Text == "" || uint.Parse(textBox.Text) <= maxValue)
            {
                prevText = textBox.Text;
                return textBox.Text;
            }
            else
            {
                return prevText;
            }
        }

        public string valdiate(TextBox textBox, string flowControllerID)
        {
            if (textBox.Text == "" || uint.Parse(textBox.Text) <= PLCService.ReadMaxValue(flowControllerID))
            {
                prevText = textBox.Text;
                return textBox.Text;
            }
            else
            {
                return prevText;
            }
        }
        private string prevText = "";
    }
    internal class FlowControllerDataGridTextColumnTextBoxValidater
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

        public string? validate(TextBox textBox, TextChangedEventArgs e)
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
            return null;
        }

        private FlowControllerTextBoxValidater get(TextBox textBox)
        {
            FlowControllerTextBoxValidater? validater = null;
            if (prevTexts.TryGetValue(textBox, out validater) == false)
            {
                validater = new FlowControllerTextBoxValidater();
                prevTexts.Add(textBox, validater);
            }
            return validater;
        }


        public string validate(TextBox textBox, string flowControllerID)
        {
            return get(textBox).valdiate(textBox, flowControllerID);
        }

        public string validate(TextBox textBox, uint maxValue)
        {
            return get(textBox).valdiate(textBox, maxValue);
        }

        private Dictionary<TextBox, FlowControllerTextBoxValidater> prevTexts = new Dictionary<TextBox, FlowControllerTextBoxValidater>();
    }
}
