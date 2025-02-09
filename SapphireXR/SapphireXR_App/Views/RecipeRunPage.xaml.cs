using CommunityToolkit.Mvvm.ComponentModel;
using SapphireXR_App.Common;
using SapphireXR_App.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SapphireXR_App.Views
{
    public partial class RecipeRunPage : Page
    {
        public RecipeRunPage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(RecipeRunViewModel));
            flowControllerDataGridTextColumnTextBoxValidater = new FlowControllerDataGridTextColumnTextBoxValidater((ObservableObject)DataContext!, "CurrentRecipe");
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
                string? validatedFlowControllerValue = flowControllerDataGridTextColumnTextBoxValidater.validate(textBox, e);
                if (validatedFlowControllerValue != null)
                {
                    textBox.Text = validatedFlowControllerValue;
                    return;
                }
            }

            throw new Exception("DataRunPage: TextBox_TextChanged must be called with TextBox in DataGridColumn whose header value has valid flow controller ");
        }

        FlowControllerDataGridTextColumnTextBoxValidater flowControllerDataGridTextColumnTextBoxValidater;
    }
}
