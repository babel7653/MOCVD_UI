using System.Windows.Controls;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Controls
{
    /// <summary>
    /// LeakTestValve.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LeakTestValve : UserControl
    {
        public LeakTestValve()
        {
            InitializeComponent();
            DataContext = new LeakTestValveViewModel();
        }

        public string ValveID
        {
            get;
            set;
        } = "";
    }
}