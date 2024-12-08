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
using Caliburn.Micro;
using TwinCAT.Ads;

namespace SapphireXR_App.Controls
{
  /// <summary>
  /// UcValve.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class UcValve : UserControl
  {
    public UcValve()
    {
      InitializeComponent();
    }

    StringBuilder eventstr = new StringBuilder();

    private void V_Checked(object sender, RoutedEventArgs e)
    {
      MessageBoxResult result = MessageBox.Show($"밸브를 여시겠습니까?", $"밸브", MessageBoxButton.OKCancel);
      if (result == MessageBoxResult.Cancel)
      {
        V01.IsChecked = false;
      }
    }

    private void V_UnChecked(object sender, RoutedEventArgs e)
    {
      MessageBoxResult result = MessageBox.Show($"밸브를 닫으시겠습니까?", $"밸브", MessageBoxButton.OKCancel);
      if (result == MessageBoxResult.Cancel)
      {
        V01.IsChecked = true;
      }
    }
  }
}
