﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Muon.View;

namespace Muon
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (Muon.Properties.Settings.Default.Auth == null)
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown;
                bool? authResult = new SettingsWindow().ShowDialog();
                if (authResult != true)
                {
                    Shutdown();
                    return;
                }
                ShutdownMode = ShutdownMode.OnLastWindowClose;
            }
            new MainWindow().Show();
        }
    }
}