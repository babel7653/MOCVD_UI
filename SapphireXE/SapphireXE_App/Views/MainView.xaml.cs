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
using System.Windows.Shapes;
using SapphireXE_App.ViewModels;

namespace SapphireXE_App.Views
{
  public partial class MainView : Window
  {
    public MainView()
    {
      InitializeComponent();

      DataContext = App.Current.Services.GetService(typeof(MainViewModel));
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      DragMove();
    }

    private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      MessageBoxResult result = MessageBox.Show("[ 예 ]:전체화면 설정/해제       [ 아니요 ] 프로그램 종료", "Windows 상태변경", MessageBoxButton.YesNoCancel);
      switch(result)
      {
        case MessageBoxResult.Yes:
          {
            if (this.WindowState == WindowState.Maximized)
            { // 이미 전체화면 -> 원래 상태
              this.WindowStyle = WindowStyle.None;
              this.WindowState = WindowState.Normal;
              // this.Topmost = false;
            }
            else
            { // 전체화면 아니면 -> 전체화면
              this.WindowStyle = WindowStyle.None;
              this.WindowState = WindowState.Maximized;
              // this.Topmost = true;
            }
          }
          break;
        case MessageBoxResult.No: Close(); break;

      }
    }
  }
}
