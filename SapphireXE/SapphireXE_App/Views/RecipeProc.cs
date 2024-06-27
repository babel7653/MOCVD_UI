using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace SapphireXE_App.Views
{
  /// <summary>
  /// RecipeControl.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class RecipeControl : Page
  {
    private void HydridCarrirerChange_Click(object sender, RoutedEventArgs e)
    {
      MessageBox.Show("밸브를 열까요?", "밸브Open", MessageBoxButton.YesNoCancel);
    }

  }
}
