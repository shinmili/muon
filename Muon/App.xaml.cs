using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Mastonet;
using Status = Mastonet.Entities.Status;
using Muon.Model;
using Muon.View;
using Muon.ViewModel;
using Reactive.Bindings;

namespace Muon
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void UpgradeSettings()
        {
            if (Muon.Properties.Settings.Default.IsSettingsFresh)
            {
                Muon.Properties.Settings.Default.Upgrade();
                Muon.Properties.Settings.Default.IsSettingsFresh = false;
                Muon.Properties.Settings.Default.Save();
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            UpgradeSettings();

            var tabsModel = new TabsModel(Muon.Properties.Settings.Default);
            var settingsViewModel = new SettingsViewModel(tabsModel);
            if (Muon.Properties.Settings.Default.Auth == null)
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown;
                bool? authResult = new SettingsWindow(settingsViewModel).ShowDialog();
                if (authResult != true)
                {
                    Shutdown();
                    return;
                }
                ShutdownMode = ShutdownMode.OnLastWindowClose;
            }
            var mainWindowViewModel = new MainWindowViewModel(new MainWindowModel(
                new ReactiveProperty<Status>(),
                new MastodonClient(Muon.Properties.Settings.Default.AppRegistration, Muon.Properties.Settings.Default.Auth),
                tabsModel));
            new MainWindow(mainWindowViewModel).Show();
        }
    }
}
