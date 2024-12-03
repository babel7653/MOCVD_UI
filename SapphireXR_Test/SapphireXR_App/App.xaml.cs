using Microsoft.Extensions.DependencyInjection;
using SapphireXR_App.Controls;
using SapphireXR_App.ViewModels;
using SapphireXR_App.Views;
using System.Windows;

namespace SapphireXR_App
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public App()
    {
      Services = ConfigureServices();
      this.InitializeComponent();
    }

    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current => (App)Application.Current;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider Services { get; }

    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    private static IServiceProvider ConfigureServices()
    {
      var services = new ServiceCollection();
      //view 등록
      services.AddTransient(typeof(MainWindow));
      services.AddTransient(typeof(HomePage));
      services.AddTransient(typeof(RecipePage));
      services.AddTransient(typeof(RecipePage));
      services.AddTransient(typeof(SettingPage));
      services.AddTransient(typeof(BottomDashboard));
      services.AddTransient(typeof(LeftDashboard));
      services.AddTransient(typeof(RightDashboard));
      services.AddTransient(typeof(FlowControlDialog));

      //ViewModel 등록
      services.AddTransient(typeof(MainViewModel));
      services.AddTransient(typeof(HomeViewModel));
      services.AddTransient(typeof(RecipeViewModel));
      services.AddTransient(typeof(ReportViewModel));
      services.AddTransient(typeof(SettingViewModel));
      services.AddTransient(typeof(BottomViewModel));
      services.AddTransient(typeof(LeftViewModel));
      services.AddTransient(typeof(RightViewModel));
      services.AddTransient(typeof(FlowControlDialogViewModel));
      //services.AddTransient(typeof(UcFlowControlViewModel));

      return services.BuildServiceProvider();
    }
  }
}
