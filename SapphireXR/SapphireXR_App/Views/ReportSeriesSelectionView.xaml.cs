using System.Windows;
using System.Windows.Input;

namespace SapphireXR_App.Views
{
    /// <summary>
    /// ReportSeriesSelectionView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReportSeriesSelectionView : Window
    {
        public ReportSeriesSelectionView()
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

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
