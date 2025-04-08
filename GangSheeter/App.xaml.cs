using GangSheeter.Services;
using GangSheeter.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace GangSheeter
{
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;

        public App()
        {
            // Setup global exception handling
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try 
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
                File.WriteAllText(logPath, $"{DateTime.Now}: {e.Exception.ToString()}");
            }
            catch { }

            MessageBox.Show($"Unhandled error: {e.Exception.Message}\n\nDetails logged to error.log", 
                "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            Shutdown();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<SheetGeneratorService>();
            services.AddSingleton<LayoutOptimizerML>();
            services.AddSingleton<ImageListViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try 
            {
                var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                try 
                {
                    string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
                    File.WriteAllText(logPath, $"{DateTime.Now}: {ex.ToString()}");
                }
                catch { }

                MessageBox.Show($"Error starting application: {ex.Message}\n\nDetails logged to error.log", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
