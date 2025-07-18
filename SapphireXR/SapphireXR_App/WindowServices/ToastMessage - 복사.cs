//using ToastNotifications.Lifetime;
//using ToastNotifications.Position;
//using ToastNotifications.Messages;
//using ToastNotifications;
//using System.Windows;
//using System.Windows.Threading;

//namespace SapphireXR_App.WindowServices
//{
//    static class ToastMessage
//    {
//        public enum MessageType { Information = 0, Sucess, Warning, Error };

//        public static void Init()
//        {
//            ToastMessageTask.Start();
//            DispatcherReadyEvent.WaitOne();
//        }
        
//        public static void Show(string message, MessageType messageType)
//        {
//            ToastMessageDispatcher?.BeginInvoke(() =>
//            {
//                switch (messageType)
//                {
//                    case MessageType.Information:
//                        notifier.ShowInformation(message);
//                        break;

//                    case MessageType.Warning:
//                        notifier.ShowWarning(message);
//                        break;

//                    case MessageType.Error:
//                        notifier.ShowError(message);
//                        break;

//                    case MessageType.Sucess:
//                        notifier.ShowSuccess(message);
//                        break;
//                }
//            });
//        }

//        private static ManualResetEvent DispatcherReadyEvent = new ManualResetEvent(false);
//        private static Thread ToastMessageTask = new Thread(() =>
//        {
//            notifier = new Notifier(cfg =>
//            {
//                cfg.PositionProvider = new WindowPositionProvider(parentWindow: Application.Current.MainWindow, corner: Corner.BottomRight, offsetX: 10, offsetY: 10);
//                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(notificationLifetime: TimeSpan.FromSeconds(3), maximumNotificationCount: MaximumNotificationCount.FromCount(5));
//                cfg.Dispatcher = ToastMessageDispatcher;
//            });

//            ToastMessageDispatcher = Dispatcher.CurrentDispatcher;
//            DispatcherReadyEvent.Set();
//            Dispatcher.Run();
//        });
//        private static Notifier? notifier = null;
//        private static Dispatcher? ToastMessageDispatcher = null;
       

//    }
//}
