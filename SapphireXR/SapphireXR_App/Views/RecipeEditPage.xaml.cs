using SapphireXR_App.Common;
using SapphireXR_App.Models;
using SapphireXR_App.ViewModels;
using SapphireXR_App.ViewModels.FlowController;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace SapphireXR_App.Views
{
    public partial class RecipeEditPage : Page
    {
        public RecipeEditPage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(RecipeEditViewModel));
            flowControllerTextBoxValidater = new FlowControllerTextBoxValidater((RecipeEditViewModel)DataContext!, "Recipes");
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Util.OnlyAllowNumber(e, e.Text);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox != null)
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
                            textBox.Text = flowControllerTextBoxValidater.valdiate(textBox, flowControllerID);
                            return;
                        }
                    }
                }
            }

            throw new Exception("DataEditPage: TextBox_TextChanged must be called with TextBox in DataGridColumn whose header value has valid flow controller ");
        }

        FlowControllerTextBoxValidater flowControllerTextBoxValidater;
    }
}
