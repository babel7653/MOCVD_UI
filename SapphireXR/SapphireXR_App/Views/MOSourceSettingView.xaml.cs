using SapphireXR_App.ViewModels;
using System.Windows;

namespace SapphireXR_App.Views
{
    /// <summary>
    /// MOSourceSettingView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MOSourceSettingView : Window
    {
        public MOSourceSettingView()
        {
            InitializeComponent();
        }

        private void moSourceDialog_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (DataContext as MOSourceSettingViewModel)?.MouseLeftButtonDownCommand.Execute(new object[2] { sender, e }); 
        }
    }
}
