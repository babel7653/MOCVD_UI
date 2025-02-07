using SapphireXR_App.ViewModels;
using System.Windows.Controls;
using System.Windows.Media;

namespace SapphireXR_App.Views
{
    public partial class RecipeRunPage : Page
    {
        public RecipeRunPage()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService(typeof(RecipeRunViewModel));                
        }
    }
}
