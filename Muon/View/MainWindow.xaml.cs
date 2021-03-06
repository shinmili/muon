﻿using Hardcodet.Wpf.TaskbarNotification;
using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Muon.Model;
using Muon.ViewModel;

namespace Muon.View
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.OnNotification += NotifyBalloon;
        }

        private void NotifyBalloon(object sender, Notification notification)
        {
            string format = "";
            switch (notification.Type)
            {
                case "follow":
                    format = "{0} has followed you";
                    break;
                case "mention":
                    format = "{0} has mentioned you";
                    break;
                case "reblog":
                    format = "{0} has reblogged your toot";
                    break;
                case "favourite":
                    format = "{0} has favourited your toot";
                    break;
            }
            string title = string.Format(format, notification.Account.DisplayName);
            string message = notification.Status?.Content ?? "";
            TaskbarIcon.ShowBalloonTip(title, message, BalloonIcon.Info);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TaskbarIcon.Dispose();
            ((MainWindowViewModel)DataContext).OnNotification -= NotifyBalloon;
        }
    }
}
