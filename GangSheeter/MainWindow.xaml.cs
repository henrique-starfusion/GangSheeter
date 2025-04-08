using GangSheeter.Services;
using GangSheeter.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace GangSheeter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Set up DI container
            var services = new ServiceCollection();
            ConfigureServices(services);
            var provider = services.BuildServiceProvider();

            // Initialize services
            var sheetGenerator = provider.GetRequiredService<SheetGeneratorService>();
            var imageList = provider.GetRequiredService<ImageListViewModel>();
            
            // Set the DataContext with dependencies
            DataContext = provider.GetRequiredService<MainViewModel>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SheetGeneratorService>();
            services.AddSingleton<ImageListViewModel>();
            services.AddSingleton<MainViewModel>();
        }
    }
}
