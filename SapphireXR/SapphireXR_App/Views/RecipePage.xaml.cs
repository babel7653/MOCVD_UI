using SapphireXR_App.ViewModels;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace SapphireXR_App.Views
{
  public partial class RecipePage : Page
  {
    public RecipePage()
    {
      InitializeComponent();
      DataContext = App.Current.Services.GetService(typeof(RecipeViewModel));
    }

    /// <summary>
    /// RecipeControl page : button controls
    /// </summary>
    #region control event
    private void ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
      if (e.VerticalChange != 0.0f)
      {
        try
        {
          ScrollViewer? sv1 = null;
          ScrollViewer? sv2 = null;

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