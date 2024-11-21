using System.Windows;
using System.Windows.Input;

namespace CustomDialogSample1.Views
{
    /// <summary>
    /// FlowControlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FlowControlView : Window
    {
        private void MessageBoxView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        public FlowControlView()
        {
            InitializeComponent();
            MouseLeftButtonDown += MessageBoxView_MouseLeftButtonDown;
        }
    }
}
