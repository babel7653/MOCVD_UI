using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using ToastNotifications.Messages;
using ToastNotifications;
using System.Windows;
using ToastNotifications.Core;

namespace SapphireXR_App.WindowServices
{
    static class ToastMessage
    {
        public enum MessageType { Information = 0, Sucess, Warning, Error };
        public static void Show(string message, MessageType messageType)
        {
            
            switch(messageType)
            {
                case MessageType.Information:
                    notifier.ShowInformation(message);
                    break;

                case MessageType.Warning:
                    notifier.ShowWarning(message);
                    break;

                case MessageType.Error:
                    notifier.ShowError(message);
                    break;

                case MessageType.Sucess:
                    notifier.ShowSuccess(message);
                    break;
            }
            
        }

        static Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(parentWindow: Application.Current.MainWindow, corner: Corner.BottomRight, offsetX: 10, offsetY: 10);
            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(notificationLifetime: TimeSpan.FromSeconds(3), maximumNotificationCount: MaximumNotificationCount.FromCount(5));
            cfg.Dispatcher = Application.Current.Dispatcher;
        });
    }
}
