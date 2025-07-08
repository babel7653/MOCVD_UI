using SapphireXR_App.Enums;
using SapphireXR_App.ViewModels;
using SapphireXR_App.Views;

namespace SapphireXR_App.WindowServices
{
    public static class FlowControlConfirmEx
    {
        public static DialogResult Show(string title, string message)
        {
            var viewModel = new ValveOperationViewModel(title, message);
            FlowControlConfirmMessageView view = new FlowControlConfirmMessageView
            {
                DataContext = viewModel
            };
            view.Topmost = true;
            view.ShowDialog();

            return viewModel.ValveOperationExResult;
        }
    }
}
