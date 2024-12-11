using SapphireXR_App.Enums;
using SapphireXR_App.ViewModels;
using SapphireXR_App.Views;

namespace SapphireXR_App.Models
{
    public static class FlowControllerEx
    {
        public static FlowControlView Show(string title, string message)
        {
            var viewModel = new FlowControlViewModel(title, message);
            FlowControlView view = new FlowControlView
            {
                DataContext = viewModel
            };
            view.Show();

            return view;
        }
    }
}
