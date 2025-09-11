using System.Windows;
using System.Windows.Input;

namespace SapphireXR_App.Views
{
    /// <summary>
    /// HeaterControlView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HeaterControlView : Window
    {
        public HeaterControlView()
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
