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


  }
}
