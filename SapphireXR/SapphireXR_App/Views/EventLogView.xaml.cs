using SapphireXR_App.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SapphireXR_App.Views
{
    /// <summary>
    /// EventLogView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EventLogView : Window
    {
        public EventLogView()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(EventLogViewModel));
            MouseLeftButtonDown += (sender, args) =>
            {
                if (args.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }
            };
        }

        private void OnHide(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
