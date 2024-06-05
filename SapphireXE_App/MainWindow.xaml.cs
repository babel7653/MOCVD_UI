using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using TwinCAT.Ads;
using System.IO;
using LiveCharts;
using LiveCharts.Wpf;

namespace SapphireXE_App
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        //Create an instance of the the TcAdsClient
        TcAdsClient tcClient;
        //Live Chart용 코드
        public SeriesCollection SeriesData { get; private set; }
        public string[] XLabel { get; set; }
        public Func<int, string> Values { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            //Live Chart 테스트 코드
            SeriesData = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "2020",
                    Values = new ChartValues<int>{3,5,7,4,7 }
                },
                new ColumnSeries
                {
                    Title ="2021",
                    Values = new ChartValues<int>{5,6,2,7,8}
                },
                new ColumnSeries
                {
                    Title = "2022",
                    Values = new ChartValues<int> { 8, 7, 6, 9, 7}
                }
            };
            XLabel = new string[] { "Kang", "Kim", "Cho", "Ko", "Song" };
            Values = value => value.ToString("N");

            DataContext = this;

        }


        private void HydridCarrirerChange_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("밸르를 열까요?", "밸브Open", MessageBoxButton.YesNoCancel);
        }
    }
}
