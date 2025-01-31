using SapphireXR_App.ViewModels.FlowController;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// FlowControllerReadOnly.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RecipeRunFlowController : UserControl
    {
        public RecipeRunFlowController()
        {
            InitializeComponent();
            DataContext = new RecipeRunFlowControllerViewModel();
        }

        public string? Type { get; set; }
        required public string ControllerID { get; set; }
    }
}
