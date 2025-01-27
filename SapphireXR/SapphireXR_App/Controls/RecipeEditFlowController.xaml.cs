using SapphireXR_App.ViewModels.FlowController;
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

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// RecipeEditFlowController.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RecipeEditFlowController : UserControl
    {
        public RecipeEditFlowController()
        {
            InitializeComponent();
            DataContext = new RecipeEditFlowControllerViewModel();
        }

        public string? Type { get; set; }
        required public string ControllerID { get; set; }
    }
}
