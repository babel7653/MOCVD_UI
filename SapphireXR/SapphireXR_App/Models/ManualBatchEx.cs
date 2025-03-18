using SapphireXR_App.Views;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Models
{
    internal class ManualBatchEx
    {
        internal static (ManualBatchViewModel.Batch?, ManualBatchViewModel.Batch?) Show()
        {
            ManualBatchViewModel manualBatchViewModel = new ManualBatchViewModel();
            ManualBatchView manualBatchView = new ManualBatchView(manualBatchViewModel);
            manualBatchView.ShowDialog();
            return (manualBatchViewModel.BatchOnAlarmState, manualBatchViewModel.BatchOnRecipeEnd);
        }
    }
}
