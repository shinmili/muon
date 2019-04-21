using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            ViewModel.NotificationStream.OnNotification += NotificationStream_OnNotification;
        }

        private void NotificationStream_OnNotification(object sender, Mastonet.StreamNotificationEventArgs e)
        {
            string format;
            switch (e.Notification.Type)
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
                default:
                    throw new NotImplementedException();
            }
            string title = string.Format(format, e.Notification.Account.DisplayName);
            string message = e.Notification.Status.Content;
            TaskbarIcon.ShowBalloonTip(title, message, BalloonIcon.Info);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TaskbarIcon.Dispose();
        }
    }
}
