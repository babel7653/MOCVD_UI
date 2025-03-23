using SapphireXR_App.ViewModels;
using SapphireXR_App.Views;

namespace SapphireXR_App.Models
{
    internal static class ReportSeriesSelectionEx
    {
       internal static void Show(ReportSeriesSelectionViewModel viewModel)
        {
            ReportSeriesSelectionView view = new ReportSeriesSelectionView() { DataContext = viewModel };
            view.ShowDialog();
        }

    }
}
