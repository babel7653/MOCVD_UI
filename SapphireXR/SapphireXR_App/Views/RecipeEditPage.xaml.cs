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
            //flowControllerDataGridTextColumnTextBoxValidater = new FlowControllerDataGridTextColumnTextBoxValidater((RecipeEditViewModel)DataContext!, "Recipes");
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
                //string? validatedFlowControllerValue = flowControllerDataGridTextColumnTextBoxValidater.validate(textBox, e);
                //if(validatedFlowControllerValue != null)
                //{
                //    textBox.Text = validatedFlowControllerValue;
                //    return;
                //}
            }

            throw new Exception("DataEditPage: TextBox_TextChanged must be called with TextBox in DataGridColumn whose header value has valid flow controller ");
        }

        //FlowControllerDataGridTextColumnTextBoxValidater flowControllerDataGridTextColumnTextBoxValidater;
    }
}
