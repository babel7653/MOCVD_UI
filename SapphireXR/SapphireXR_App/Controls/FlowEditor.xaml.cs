using System.Windows;
using System.Windows.Controls;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Controls
{
    public partial class FlowEditor : UserControl
    {   
        public FlowEditor()
        {
            InitializeComponent();
            DataContext = new FlowEditorViewModel();
        }

        public string ContentLabel { get; set; } = "";
    }
}
