using SapphireXR_App.ViewModels;
using SapphireXR_App.Views;

namespace SapphireXR_App.WindowServices
{
    internal static class ReportSeriesSelectionEx
    {
       internal static IList<string> Show(ReportSeriesSelectionViewModel viewModel)
        {
            ReportSeriesSelectionView view = new ReportSeriesSelectionView() { DataContext = viewModel };
            view.ShowDialog();

            return viewModel.SelectedNames;
        }

    }
}
