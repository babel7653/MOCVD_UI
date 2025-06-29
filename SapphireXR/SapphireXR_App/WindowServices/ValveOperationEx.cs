using SapphireXR_App.Enums;
using SapphireXR_App.ViewModels;
using SapphireXR_App.Views;

namespace SapphireXR_App.WindowServices
{
    public static class ValveOperationEx
    {
        public static ValveOperationExResult Show(string title, string message)
        {
            var viewModel = new ValveOperationViewModel(title, message);
            ValveOperationView view = new ValveOperationView
            {
                DataContext = viewModel
            };
            view.ShowDialog();

            return viewModel.ValveOperationExResult;
        }
    }
}
