using SapphireXR_App.Models;
using SapphireXR_App.ViewModels;
using System.Windows;

namespace SapphireXR_App.Views
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      DataContext = App.Current.Services.GetService(typeof(MainViewModel));
    }

    private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      DragMove();

    }

    private void Window_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      MessageBoxResult result = MessageBox.Show("[ 예 ]:전체화면 설정/해제       [ 아니요 ] 프로그램 종료", "Windows 상태변경", MessageBoxButton.YesNoCancel);
      switch (result)
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
