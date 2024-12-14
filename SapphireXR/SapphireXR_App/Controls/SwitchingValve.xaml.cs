using System.Windows;
using System.Windows.Controls;
using SapphireXR_App.Enums;
using SapphireXR_App.Models;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Controls
{
    public partial class SwitchingValve : UserControl
    {
        public SwitchingValve()
        {
            InitializeComponent();
            DataContext = new SwitchingValveViewModel();
        }

        public string ValveID
        {
            get;
            set;
        } = "";
       
    }
}
