using SapphireXR_App.ViewModels;
using SapphireXR_App.Views;
using System.Windows;

namespace SapphireXR_App.WindowServices
{
    public static class FlowControllerEx
    {
        public static Window Show(string title, string message, string controllerID)
        {
            Window view;
            if (controllerID == "Pressure")
            {
                view = new PressControlView { DataContext = new PressControlViewModel(title, message, controllerID) };
            }
            else if (controllerID == "Temperature")
            {
                view = new HeaterControlView { DataContext = new HeaterControlViewModel(title, message, controllerID) };
            }
            else
            {
                view = new FlowControlView { DataContext = new FlowControlViewModel(title, message, controllerID) };
            }

            view.Show();

            return view;
        }
    }
}
