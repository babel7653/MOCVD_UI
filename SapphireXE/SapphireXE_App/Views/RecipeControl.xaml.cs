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

      RecipeData recipe = new RecipeData();
      DataContext = recipe;

      RecipeStepReactor.ItemsSource = recipe.recipeData;
      RecipeStepMFC.ItemsSource = recipe.recipeData;
      RecipeStepValve.ItemsSource = recipe.recipeData;

    }

    private void RecipeStart_Click(object sender, RoutedEventArgs e)
    {

      MessageBox.Show("Recipe start");
    }
  }
}
