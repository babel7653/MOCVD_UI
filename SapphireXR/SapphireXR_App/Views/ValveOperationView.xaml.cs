using System.Windows;
using System.Windows.Input;

namespace SapphireXR_App.Views
{
    /// <summary>
    /// ValveOperationView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ValveOperationView : Window
    {
        private void MessageBoxView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        public ValveOperationView()
        {
            InitializeComponent();
            MouseLeftButtonDown += MessageBoxView_MouseLeftButtonDown;
        }
    }
}
