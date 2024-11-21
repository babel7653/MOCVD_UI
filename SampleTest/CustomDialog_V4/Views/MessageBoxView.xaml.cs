using System.Windows;
using System.Windows.Input;

namespace CustomDialogSample1.Views
{
    /// <summary>
    /// MessageBoxView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageBoxView : Window
    {
        private void MessageBoxView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        public MessageBoxView()
        {
            InitializeComponent();
            MouseLeftButtonDown += MessageBoxView_MouseLeftButtonDown;
        }
    }
}
