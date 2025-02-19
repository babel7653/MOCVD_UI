using SapphireXR_App.Views;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Models
{
    internal class ManualBatchEx
    {
        internal static void Show()
        {
            ManualBatchView manualBatchView = new ManualBatchView();
            manualBatchView.DataContext = new ManualBatchViewModel();
            manualBatchView.ShowDialog();
        }
    }
}
