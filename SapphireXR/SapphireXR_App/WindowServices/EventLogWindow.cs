using SapphireXR_App.Views;

namespace SapphireXR_App.WindowServices
{
    internal class EventLogWindow
    {
        public static void Show()
        {
            if (EventLogViewWindow == null)
            {
                EventLogViewWindow = new EventLogView() {  Topmost = true };
                EventLogViewWindow.Closed += (sender, args) =>
                {
                    EventLogViewWindow = null;
                };
            }
            EventLogViewWindow.Show();
        }

        private static EventLogView? EventLogViewWindow = null;
    }
}
