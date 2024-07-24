using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SapphireXE_App.ViewModels;

namespace SapphireXE_App
{
  /// <summary>
  /// App.xaml에 대한 상호 작용 논리
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

      //services.AddSingleton<IFilesService, FilesService>();
      //services.AddSingleton<ISettingsService, SettingsService>();
      //services.AddSingleton<IClipboardService, ClipboardService>();
      //services.AddSingleton<IShareService, ShareService>();
      //services.AddSingleton<IEmailService, EmailService>();
      services.AddTransient(typeof(MainViewModel));
      services.AddTransient(typeof(SystemControlViewModel));
      services.AddTransient(typeof(RecipeControlViewModel));
      services.AddTransient(typeof(ReportViewModel));
      services.AddTransient(typeof(SettingViewModel));

      return services.BuildServiceProvider();
    }
  }
}
