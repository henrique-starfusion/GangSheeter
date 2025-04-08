using GangSheeter.Services;
using GangSheeter.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace GangSheeter
{
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<SheetGeneratorService>();
            services.AddSingleton<ImageListViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
