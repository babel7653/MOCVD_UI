using System.Windows;
using System.Windows.Input;

namespace SapphireXR_App.Views
{
    /// <summary>
    /// PressControlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PressControlView : Window
    {
        public PressControlView()
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
