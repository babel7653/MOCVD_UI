using System.Windows;
using System.Windows.Controls;

namespace CustomDialogSample1.Controls
{
    /// <summary>
    /// FlowControllerControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FlowControllerControl : UserControl
    {
        public FlowControllerControl()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtInput.Focus();
        }
    }
}
