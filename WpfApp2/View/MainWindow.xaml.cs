using Hardcodet.Wpf.TaskbarNotification;
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
using WpfApp2.Model;
using WpfApp2.ViewModel;

namespace WpfApp2.View
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var vm = new MainViewModel(new MainModel(
                new ReactiveProperty<Status>(),
                new MastodonClient(Properties.Settings.Default.AppRegistration, Properties.Settings.Default.Auth),
                new TabsModel(Properties.Settings.Default.Tabs)));
            DataContext = vm;
            (vm.Notifications as INotifyCollectionChanged).CollectionChanged += MainWindow_CollectionChanged;
        }

        private void MainWindow_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            string format = "";
            var notification = (Notification)e.NewItems[0];
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
        }
    }
}
