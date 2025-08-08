using System.Windows;
using System.Windows.Input;

namespace SapphireXR_App.Views
{
    /// <summary>
    /// TriggeredWarningAlarmView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TriggeredWarningAlarmView : Window
    {
        public TriggeredWarningAlarmView()
        {
            InitializeComponent();
            MouseLeftButtonDown += (sender, args) =>
            {
                if (args.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }
            };
        }
    }
}
