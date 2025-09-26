using SapphireXR_App.Common;
using SapphireXR_App.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SapphireXR_App.Views
{
    public class RecipeLoopReadOnlyBindingProxy : Freezable
    {
        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore()
        {
            return new RecipeLoopReadOnlyBindingProxy();
        }

        #endregion

        public object DataContext
        {
            get { return (object)GetValue(DataContextProperty); }
            set { SetValue(DataContextProperty, value); }
        }

        public static readonly DependencyProperty DataContextProperty =
            DependencyProperty.Register("DataContext", typeof(object),
                                         typeof(RecipeLoopReadOnlyBindingProxy));
    }

    public partial class RecipeRunPage : Page
    {
        public RecipeRunPage()
        {
            InitializeComponent();
            RecipeRunViewModel viewModel = (RecipeRunViewModel)(DataContext = App.Current.Services.GetService(typeof(RecipeRunViewModel)))!;
            flowControllerDataGridTextColumnTextBoxValidaterMaxValue = new FlowControllerDataGridTextColumnTextBoxValidaterMaxValue(viewModel, nameof(viewModel.CurrentRecipe));
            flowControllerDataGridTextColumnTextBoxValidaterOnlyNumber = new FlowControllerDataGridTextColumnTextBoxValidaterOnlyNumber(viewModel, nameof(viewModel.CurrentRecipe));
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
            Util.ConstraintEmptyToDefaultValueOnDataGridCellCommit(sender, e, ColumnDefaultValue);
        }

        FlowControllerDataGridTextColumnTextBoxValidaterMaxValue flowControllerDataGridTextColumnTextBoxValidaterMaxValue;
        FlowControllerDataGridTextColumnTextBoxValidaterOnlyNumber flowControllerDataGridTextColumnTextBoxValidaterOnlyNumber;

        private static readonly Dictionary<string, string> ColumnDefaultValue = new Dictionary<string, string>() { { "Ramp", "1" }, { "Hold", "1" } };
    }
}
