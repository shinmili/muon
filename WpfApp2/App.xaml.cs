using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.View;

namespace WpfApp2
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (WpfApp2.Properties.Settings.Default.Auth == null)
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown;
                bool? authResult = new SettingsWindow().ShowDialog();
                if (!authResult.HasValue || authResult.Value == false)
                {
                    Shutdown();
                }
            }
            Window w = new MainWindow();
            w.Show();
            ShutdownMode = ShutdownMode.OnLastWindowClose;
        }
    }
}
