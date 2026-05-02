using System.Windows;
using Velopack;

namespace SchedulerAdminUI
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            VelopackApp.Build().Run();

            base.OnStartup(e);

            var splash = new SplashWindow();
            splash.Show();

            await Task.Delay(4200);

            var mainWindow = new MainWindow();
            mainWindow.Show();

            splash.Close();
        }
    }
}