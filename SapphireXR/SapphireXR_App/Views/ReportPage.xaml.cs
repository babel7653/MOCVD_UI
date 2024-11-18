using System.Windows.Controls;
using SapphireXR_App.ViewModels;

namespace SapphireXR_App.Views
{
  public partial class ReportPage : Page
  {
    public ReportPage()
    {
      InitializeComponent();

      DataContext = App.Current.Services.GetService(typeof(ReportViewModel));
    }
  }
}
