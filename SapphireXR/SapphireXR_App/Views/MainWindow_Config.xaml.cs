using SapphireXR_App.ViewModels;
using System.Windows;

namespace SapphireXR_App.Views
{
    /// <summary>
    /// MainWindow_Config.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow_Config : Window
    {
        public MainWindow_Config()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(MainViewModel));
        }
    }
}
