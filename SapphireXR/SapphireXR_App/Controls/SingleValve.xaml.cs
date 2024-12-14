using System.Windows.Controls;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// SingleValve.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SingleValve : UserControl
    {
        public SingleValve()
        {
            InitializeComponent();
            DataContext = new SingleValveViewModel();
        }

        public string ValveID
        {
            get;
            set;
        } = "";
    }
}