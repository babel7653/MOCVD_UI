using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using Microsoft.Win32;
using SapphireXE_App.ViewModels;
using SapphireXE_App.Models;
using SapphireXE_App;

namespace SapphireXE_App.Views
{
  /// <summary>
  /// RecipeControl.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class RecipeControl : Page
  {
    public RecipeControl()
    {
      InitializeComponent();

      DataContext = App.Current.Services.GetService(typeof(RecipeControlViewModel));
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
