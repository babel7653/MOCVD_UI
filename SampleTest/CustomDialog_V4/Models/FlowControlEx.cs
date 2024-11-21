using CustomDialogSample1.ViewModels;
using CustomDialogSample1.Views;

namespace CustomDialogSample1.Models
{
    public class FlowControlEx
    {
        public static string? Show(string title, string prompt, string defaultInputMessage = "")
        {
            FlowControlViewModel viewModel = new FlowControlViewModel(title, prompt, defaultInputMessage);
            FlowControlView view = new FlowControlView
            {
                DataContext = viewModel
            };

            return view.ShowDialog() ?? false
              ? viewModel.Response
              : null;
        }
    }
}
