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
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
                            Util.OnlyAllowConstrainedNumber(e, textBox.Text, e.Text, (int)PLCService.ReadMaxValue(flowControllerID));
                        }
                    }
                }
            }
        }
    }
}
