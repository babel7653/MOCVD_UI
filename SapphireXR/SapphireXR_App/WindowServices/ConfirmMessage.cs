using SapphireXR_App.Enums;
using SapphireXR_App.ViewModels;
using SapphireXR_App.Views;
using System.Windows;

namespace SapphireXR_App.WindowServices
{
    public static class ConfirmMessage
    {
        public static ValveOperationExResult Show(string title, string message, WindowStartupLocation windowStartupLocation)
        {
            var viewModel = new ValveOperationViewModel(title, message);
            ConfirmMessageView view = new ConfirmMessageView
            {
                DataContext = viewModel,
                WindowStartupLocation = windowStartupLocation
            };
            view.Topmost = true;
            view.ShowDialog();

            return viewModel.ValveOperationExResult;
        }
    }
}
