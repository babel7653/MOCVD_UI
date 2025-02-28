using System.Windows;
using System.Windows.Controls;
using SapphireXR_App.Common;
using SapphireXR_App.Models;
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
            //flowControllerDataGridTextColumnTextBoxValidater = new FlowControllerDataGridTextColumnTextBoxValidater(viewModel, nameof(viewModel.CurrentBatch));
        }

        //private FlowControllerDataGridTextColumnTextBoxValidater flowControllerDataGridTextColumnTextBoxValidater;

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Util.OnlyAllowNumber(e, e.Text);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox != null)
            {
                ManualBatchViewModel.AnalogIOUserState? dataContext = textBox.DataContext as ManualBatchViewModel.AnalogIOUserState;
                if (dataContext != null)
                {
                    //textBox.Text = flowControllerDataGridTextColumnTextBoxValidater.validate(textBox, (uint)dataContext.MaxValue);
                }
            }
           
        }
    }
}
