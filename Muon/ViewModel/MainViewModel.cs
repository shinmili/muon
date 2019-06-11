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
using Muon.Model;
using Muon.View;

namespace Muon.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
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
        public ReactiveCommand<TabContentViewModelBase> CloseTabCommand { get; }
        public ReactiveCommand PrevTabCommand { get; }
        public ReactiveCommand NextTabCommand { get; }

        public MainViewModel(MainModel model)
        {
            this.model = model;
            NewTootBoxViewModel = new NewTootBoxViewModel(this.model.InReplyTo, this.model.Client);
            TabViewModels = this.model.Tabs.ToReadOnlyReactiveCollection(p => TabContentViewModelBase.FromParam(p, this.model.InReplyTo, this.model.Tabs, this.model.Client));
            SelectedTabIndex = this.model.Tabs.SelectedIndex;
            Notifications = TabViewModels.OfType<NotificationsViewModel>().FirstOrDefault().Notifications;

            OpenSettingsCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    var w = new SettingsWindow();
                    ((SettingsViewModel)w.DataContext).Tabs = this.model.Tabs;
                    w.ShowDialog();
                });
            CloseTabCommand = new ReactiveCommand<TabContentViewModelBase>()
                .WithSubscribe(vm =>
                {
                    this.model.Tabs.CloseTab(TabViewModels.IndexOf(vm));
                });
            NextTabCommand = new ReactiveCommand()
                .WithSubscribe(this.model.Tabs.SwitchToNextTab);
            PrevTabCommand = new ReactiveCommand()
                .WithSubscribe(this.model.Tabs.SwitchToPrevTab);
        }
    }
}
