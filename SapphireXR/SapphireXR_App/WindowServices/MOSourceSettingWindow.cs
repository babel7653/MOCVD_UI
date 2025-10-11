using SapphireXR_App.Views;
using System.Windows;

namespace SapphireXR_App.WindowServices
{
    internal static class MOSourceSettingWindow
    {
        internal static Window Show(ViewModels.MOSourceSettingViewModel moSourceSettingViewModel)
        {
            MOSourceSettingView moSettingWindow = new MOSourceSettingView() { DataContext = moSourceSettingViewModel };
            moSettingWindow.Show();

            return moSettingWindow;
        }
    }
}
