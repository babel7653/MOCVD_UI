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
        internal FlowControllerTextBoxValidater(ObservableObject viewModel, string recipesPropertyName)
        {
            viewModel.PropertyChanged += (object? sender, PropertyChangedEventArgs args) =>
            {
                if (args.PropertyName == recipesPropertyName)
                {
                    prevTexts.Clear();
                }
            };
        }

        public string valdiate(TextBox textBox, string flowControllerID)
        {
            if (textBox.Text == "" || uint.Parse(textBox.Text) <= PLCService.ReadMaxValue(flowControllerID))
            {
                prevTexts[textBox] = textBox.Text;
                return textBox.Text;
            }
            else
            {
                return prevTexts[textBox];
            }
        }
        private Dictionary<TextBox, string> prevTexts = new Dictionary<TextBox, string>();
    }
}
