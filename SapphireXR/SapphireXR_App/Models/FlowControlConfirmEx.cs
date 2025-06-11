using SapphireXR_App.Enums;
using SapphireXR_App.ViewModels;
using SapphireXR_App.Views;

namespace SapphireXR_App.Models
{
    public static class FlowControlConfirmEx
    {
        public static ValveOperationExResult Show(string title, string message)
        {
            var viewModel = new ValveOperationViewModel(title, message);
            ValveOperationView view = new ValveOperationView
            {
                DataContext = viewModel
            };
            view.Topmost = true;
            view.ShowDialog();

            return viewModel.ValveOperationExResult;
        }
    }
}
