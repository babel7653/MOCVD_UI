﻿using SapphireXR_App.Common;
using SapphireXR_App.ViewModels;
using System.Windows.Controls;

namespace SapphireXR_App.Views
{
    public partial class RecipeEditPage : Page
    {
        public RecipeEditPage()
        {
            InitializeComponent();
            RecipeEditViewModel viewModel = (RecipeEditViewModel)(DataContext = App.Current.Services.GetService(typeof(RecipeEditViewModel)))!;
            flowControllerDataGridTextColumnTextBoxValidaterMaxValue = new FlowControllerDataGridTextColumnTextBoxValidaterMaxValue(viewModel, nameof(viewModel.Recipes));
            flowControllerDataGridTextColumnTextBoxValidaterOnlyNumber = new FlowControllerDataGridTextColumnTextBoxValidaterOnlyNumber(viewModel, nameof(viewModel.Recipes));
        }

        private void TextBox_TextChangedMaxNumber(object sender, TextChangedEventArgs e)
        {
            Util.CostraintTextBoxColumnMaxNumber(sender, flowControllerDataGridTextColumnTextBoxValidaterMaxValue, e);
        }

        private void TextBox_TextChangedOnlyNumber(object sender, TextChangedEventArgs e)
        {

            Util.CostraintTextBoxColumnOnlyNumber(sender, flowControllerDataGridTextColumnTextBoxValidaterOnlyNumber);
        }

        private void flowDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Util.ConstraintEmptyToZeroOnDataGridCellCommitForRecipeRunEdit(sender, e);
        }

        FlowControllerDataGridTextColumnTextBoxValidaterMaxValue flowControllerDataGridTextColumnTextBoxValidaterMaxValue;
        FlowControllerDataGridTextColumnTextBoxValidaterOnlyNumber flowControllerDataGridTextColumnTextBoxValidaterOnlyNumber;
    }
}
