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

namespace SapphireXE_App
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        //Create an instance of the the TcAdsClient
        TcAdsClient tcClient;
        public MainWindow()
        {
            InitializeComponent();
        }


        private void HydridCarrirerChange_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("밸르를 열까요?", "밸브Open", MessageBoxButton.YesNoCancel);
        }
    }
}
