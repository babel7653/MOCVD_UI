﻿using Microsoft.Extensions.DependencyInjection;
using SapphireXR_App.ViewModels;
using SapphireXR_App.Views;
using System.Windows;

namespace SapphireXR_App
{
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
            Startup += App_Startup;
        }
        private void App_Startup(object sender, StartupEventArgs e)
        {
            // 생성자 주입 구문을 사용하면 매개변수를 입력하지 않아도 객체가 만들어 지고 호출이 가능
            // 단, 이것도 서비스에 등록이 되어야 함
            var mainView = App.Current.Services.GetService<MainWindow>();
            if (mainView != null)
            {
                mainView.Show();
            }
            else
            {
                Shutdown();
            }
        }
        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            /*
           Services
           Singleton으로 선언하여 TestService객체를 ITestService 인터페이스로 서비스 주입
           의존성을 주입하면 main에서 생성자에서 불러 올 수 있음
           services.AddSingleton<ITestService, TestService>();
           한번 생성하면 그 객체는 끝날때까지 사용
           Viewmodels
           의존성이 추가된 MainView를 만듬
           */
            services.AddSingleton<MainWindow>();
            services.AddSingleton<HomePage>();

            services.AddTransient(typeof(MainViewModel));
            services.AddTransient(typeof(HomeViewModel));
            services.AddTransient(typeof(RecipeViewModel));
            services.AddTransient(typeof(ReportViewModel));
            services.AddTransient(typeof(BottomViewModel));
            services.AddTransient(typeof(LeftViewModel));
            services.AddTransient(typeof(RightViewModel));
            services.AddTransient(typeof(SettingViewModel));

            return services.BuildServiceProvider();
        }
    }
}