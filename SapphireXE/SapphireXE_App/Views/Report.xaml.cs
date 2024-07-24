using System.Windows.Controls;
using SapphireXE_App.ViewModels;

namespace SapphireXE_App.Views
{
  public partial class Report : Page
  {
    public Report()
    {
      InitializeComponent();

      DataContext = App.Current.Services.GetService(typeof(ReportViewModel));
    }
  }
}
