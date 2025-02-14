using SapphireXR_App.ViewModels;
using System.ComponentModel;
using System.Windows;

namespace SapphireXR_App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(MainViewModel));
        }

        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {

        }
    }
}
