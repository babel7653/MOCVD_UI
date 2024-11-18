using Microsoft.Extensions.DependencyInjection;
using SapphireXR_App.ViewModels;
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
    public new static App Current => (App)Application.Current;
    public IServiceProvider Services { get; }

    private static IServiceProvider ConfigureServices()
    {
      var services = new ServiceCollection();

      //ViewModel 등록
      services.AddTransient(typeof(MainViewModel));
      services.AddTransient(typeof(HomeViewModel));
      services.AddTransient(typeof(RecipeViewModel));
      services.AddTransient(typeof(ReportViewModel));
      services.AddTransient(typeof(SettingViewModel));
      services.AddTransient(typeof(BottomViewModel));
      services.AddTransient(typeof(LeftViewModel));
      services.AddTransient(typeof(RightViewModel));
      services.AddTransient(typeof(AnalogDeviceControlViewModel));

      return services.BuildServiceProvider();
    }
  }
}
