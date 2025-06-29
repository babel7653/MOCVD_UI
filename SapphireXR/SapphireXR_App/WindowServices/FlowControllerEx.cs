using SapphireXR_App.ViewModels;
using SapphireXR_App.Views;

namespace SapphireXR_App.WindowServices
{
    public static class FlowControllerEx
    {
        public static FlowControlView Show(string title, string message, string controllerID, FlowControlViewModel.ConfirmedEventHandler onConfirmed, FlowControlViewModel.CanceledEventHandler onCanceled)
        {
            var viewModel = new FlowControlViewModel(title, message, controllerID);
            viewModel.Confirmed += onConfirmed;
            viewModel.Canceled += onCanceled;
            FlowControlView view = new FlowControlView
            {
                DataContext = viewModel
            };
            view.Show();

            return view;
        }
    }
}
