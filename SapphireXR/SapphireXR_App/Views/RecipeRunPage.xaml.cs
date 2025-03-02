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
            flowControllerDataGridTextColumnTextBoxValidaterMaxValue = new FlowControllerDataGridTextColumnTextBoxValidaterMaxValue((ObservableObject)DataContext!, "CurrentRecipe");
            flowControllerDataGridTextColumnTextBoxValidaterOnlyNumber = new FlowControllerDataGridTextColumnTextBoxValidaterOnlyNumber((ObservableObject)DataContext!, "CurrentRecipe");
        }

        private void TextBox_TextChangedMaxNumber(object sender, TextChangedEventArgs e)
        {
            Util.CostraintTextBoxColumnMaxNumber(sender, flowControllerDataGridTextColumnTextBoxValidaterMaxValue, e);
        }

        private void TextBox_TextChangedOnlyNumber(object sender, TextChangedEventArgs e)
        {
            
             Util.CostraintTextBoxColumnOnlyNumber(sender, flowControllerDataGridTextColumnTextBoxValidaterOnlyNumber);
        }
      
        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Util.ConstraintEmptyToZeroOnDataGridCellCommitForRecipeRunEdit(sender, e);
        }

        FlowControllerDataGridTextColumnTextBoxValidaterMaxValue flowControllerDataGridTextColumnTextBoxValidaterMaxValue;
        FlowControllerDataGridTextColumnTextBoxValidaterOnlyNumber flowControllerDataGridTextColumnTextBoxValidaterOnlyNumber;

    }
}
