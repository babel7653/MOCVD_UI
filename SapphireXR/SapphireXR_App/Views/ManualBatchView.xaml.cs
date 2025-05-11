using System.Windows;
using System.Windows.Controls;
using SapphireXR_App.Common;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Views
{
    /// <summary>
    /// ManualBatchView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ManualBatchView : Window
    {
        public ManualBatchView(ManualBatchViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            flowControllerDataGridTextColumnTextBoxValidaterMaxValue = new FlowControllerDataGridTextColumnTextBoxValidaterMaxValue(viewModel, nameof(viewModel.CurrentBatch));
        }

        private FlowControllerDataGridTextColumnTextBoxValidaterMaxValue flowControllerDataGridTextColumnTextBoxValidaterMaxValue;

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox != null)
            {
                ManualBatchViewModel.AnalogIOUserState? dataContext = textBox.DataContext as ManualBatchViewModel.AnalogIOUserState;
                if (dataContext != null)
                {
                    Util.CostraintTextBoxColumnMaxNumber(textBox, flowControllerDataGridTextColumnTextBoxValidaterMaxValue, (uint)dataContext.MaxValue);
                }
            }
        }

        private void dgUserSettingA_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Util.ConstraintEmptyToZeroOnDataGridCellCommit(sender, e, ["Value"]);
        }

        private void NumberBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox? textBox = e.OriginalSource as TextBox;
            if(textBox != null && textBox.Text == "")
            {
                textBox.Text = "0";
            }
        }

        private void dgUserSettingD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
