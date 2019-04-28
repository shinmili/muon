using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Model;
using WpfApp2.View;

namespace WpfApp2.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private MainModel model;

        public NewTootBoxViewModel NewTootBoxViewModel { get; }

        public ReactiveProperty<Status> InReplyTo => model.InReplyTo;
        public ReadOnlyReactiveCollection<TabContentViewModelBase> TabViewModels { get; }
        public ReactiveProperty<TabContentViewModelBase> SelectedTab { get; } = new ReactiveProperty<TabContentViewModelBase>();
        public ReactiveProperty<int> SelectedTabIndex { get; }
        public ReadOnlyObservableCollection<Notification> Notifications { get; }

        public ReactiveCommand OpenSettingsCommand { get; }
        public ReactiveCommand NewTabCommand { get; }
        public ReactiveCommand<TabContentViewModelBase> CloseTabCommand { get; }
        public ReactiveCommand PrevTabCommand { get; }
        public ReactiveCommand NextTabCommand { get; }

        private TabSettingsModel tabs = TabSettingsModel.Default;

        public MainViewModel(MainModel model)
        {
            this.model = model;
            NewTootBoxViewModel = new NewTootBoxViewModel(this.model.InReplyTo);
            TabViewModels = tabs.ToReadOnlyReactiveCollection(p => TabContentViewModelBase.FromParam(p, this.model.InReplyTo));
            SelectedTabIndex = tabs.SelectedIndex;
            Notifications = TabViewModels.OfType<NotificationsViewModel>().FirstOrDefault().Notifications;

            OpenSettingsCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    var w = new SettingsWindow();
                    ((SettingsViewModel)w.DataContext).Tabs = tabs;
                    w.ShowDialog();
                });
            NewTabCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    var w = new NewTabWindow();
                    ((NewTabViewModel)w.DataContext).Tabs = tabs;
                    w.ShowDialog();
                });
            CloseTabCommand = new ReactiveCommand<TabContentViewModelBase>()
                .WithSubscribe(vm =>
                {
                    tabs.RemoveAt(TabViewModels.IndexOf(vm));
                });
            NextTabCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    SelectedTabIndex.Value = (SelectedTabIndex.Value + 1) % tabs.Count;
                });
            PrevTabCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    SelectedTabIndex.Value = (SelectedTabIndex.Value + tabs.Count() - 1) % tabs.Count;
                });
        }
    }
}
