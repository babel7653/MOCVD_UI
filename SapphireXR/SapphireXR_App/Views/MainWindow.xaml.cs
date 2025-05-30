using SapphireXR_App.ViewModels;
using System.Windows;
using System.ComponentModel;

namespace SapphireXR_App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(MainViewModel));
        }

        private void OnMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            CancelEventArgs cancelEventArgs = new CancelEventArgs();
            ((MainViewModel)DataContext).OnClosingCommand.Execute(cancelEventArgs);
            if (cancelEventArgs.Cancel == false)
            {
                App.Current.Shutdown();
            }
        }
    }
}
