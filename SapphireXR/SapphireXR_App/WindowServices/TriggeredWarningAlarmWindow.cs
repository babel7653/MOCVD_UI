using SapphireXR_App.Models;
using SapphireXR_App.ViewModels;
using SapphireXR_App.Views;

namespace SapphireXR_App.WindowServices
{
    internal static class TriggeredWarningAlarmWindow
    {
        internal static void Show(PLCService.TriggerType type)
        {
            new TriggeredWarningAlarmView()
            {
                DataContext = new TriggeredWarningAlarmViewModel(type),
                Topmost = true
            }.Show();
        }
    }
}
