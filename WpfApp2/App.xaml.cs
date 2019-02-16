using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var settings = new SettingsModel();
            Window w;
            if (string.IsNullOrEmpty(settings.Auth?.CreatedAt))
            {
                w = new AuthorizationWindow();
            }
            else
            {
                w = new MainWindow();
            }
            w.Show();
        }
    }
}
