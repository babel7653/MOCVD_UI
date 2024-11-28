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
using SapphireXE_App.Models;
using System.Reflection;
using SapphireXE_App.Enums;

namespace SapphireXE_App.Views
{
  public partial class MainView : Window
  {
    public MainView()
    {
      InitializeComponent();

      DataContext = App.Current.Services.GetService(typeof(MainViewModel));

      // settings
      comboSystemStart.ItemsSource = Enum.GetValues(typeof(EUserState)).Cast<EUserState>();
      comboAlarmStart.ItemsSource = Enum.GetValues(typeof(EUserState)).Cast<EUserState>();
      comboRecipeEnd.ItemsSource = Enum.GetValues(typeof(EUserState)).Cast<EUserState>();

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


    private void ValveOperationViewPopup(object sender, RoutedEventArgs e)
    {
        var result = ValveOperationEx.Show("Valve Operation", "밸브를 여시겠습니까?");
        switch (result)
        {
            case ValveOperationExResult.Ok:
                MessageBox.Show("밸브 열음");
                break;
            case ValveOperationExResult.Canel:

                MessageBox.Show("취소됨");
                break;
                //TODO : 밸브 열림/닫힘 상태 확인 후 메세지 뛰우고 닫혔을 때 -> 열고, 열렸을 때 -> 담음
        }
    }

        /// <summary>
        /// RecipeControl page : button controls
        /// </summary>
        #region control event

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if (e.VerticalChange != 0.0f)
      {
        ScrollViewer sv1 = null;
        ScrollViewer sv2 = null;

        try
        {
          if (sender.Equals(RecipeStepReactor))
          {
            Type t = RecipeStepReactor.GetType();
            sv1 = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance |
                BindingFlags.GetProperty, null, RecipeStepMFC, null) as ScrollViewer;
            sv2 = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance |
                BindingFlags.GetProperty, null, RecipeStepValve, null) as ScrollViewer;

          }
          else if (sender.Equals(RecipeStepMFC))
          {
            Type t = RecipeStepMFC.GetType();
            sv1 = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance |
                BindingFlags.GetProperty, null, RecipeStepReactor, null) as ScrollViewer;
            sv2 = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance |
                BindingFlags.GetProperty, null, RecipeStepValve, null) as ScrollViewer;
          }
          else
          {
            Type t = RecipeStepValve.GetType();
            sv1 = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance |
                BindingFlags.GetProperty, null, RecipeStepReactor, null) as ScrollViewer;
            sv2 = t.InvokeMember("InternalScrollHost", BindingFlags.NonPublic | BindingFlags.Instance |
                BindingFlags.GetProperty, null, RecipeStepMFC, null) as ScrollViewer;
          }
          sv1?.ScrollToVerticalOffset(e.VerticalOffset);
          sv2?.ScrollToVerticalOffset(e.VerticalOffset);
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
        }

      }
    }

    #endregion
  }
}
