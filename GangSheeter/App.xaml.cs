using System.Windows;
using Application = System.Windows.Application;
using GangSheeter.Data;

namespace GangSheeter
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Initialize database
            using var db = new AppDbContext();
            db.Database.EnsureCreated();
        }
    }
}
