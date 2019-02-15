using Mastonet;
using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SettingsModel settings = new SettingsModel();
        private MastodonClient client;

        public MainViewModel()
        {
            client = new MastodonClient(settings.AppRegistration, settings.Auth);
            Home = new ObservableCollection<Status>();
        }

        #region INotifyPropertyChanged implementations

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string text = "";
        public string Text
        {
            get => text;
            set
            {
                text = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<Status> home;
        public ObservableCollection<Status> Home
        {
            get => home;
            set
            {
                home = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Commands

        private DelegateCommand tootCommand;
        public DelegateCommand TootCommand
        {
            get => tootCommand ?? (tootCommand = new DelegateCommand
            {
                ExecuteHandler = executeTootCommand,
                CanExecuteHandler = null
            });
        }

        private void executeTootCommand(object parameter)
        {

        }
        private DelegateCommand reloadCommand;
        public DelegateCommand ReloadCommand
        {
            get => reloadCommand ?? (reloadCommand = new DelegateCommand
            {
                ExecuteHandler = executeReloadCommand,
                CanExecuteHandler = null
            });
        }

        private async void executeReloadCommand(object parameter)
        {
            var home = await client.GetHomeTimeline();
            home.Reverse();
            foreach (Status status in home)
            {
                Home.Insert(0, status);
            }
        }

        #endregion
    }
}
