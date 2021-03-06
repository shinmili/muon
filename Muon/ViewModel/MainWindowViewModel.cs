﻿using Mastonet;
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
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<Notification> OnNotification
        {
            add { TabViewModels.OfType<NotificationsViewModel>().SingleOrDefault().OnNotification += value; }
            remove { TabViewModels.OfType<NotificationsViewModel>().SingleOrDefault().OnNotification -= value; }
        }

        private MainWindowModel model;

        public NewTootBoxViewModel NewTootBoxViewModel { get; }

        public ReactiveProperty<Status> InReplyTo => model.InReplyTo;
        public ReadOnlyReactiveCollection<TabContentViewModelBase> TabViewModels { get; }
        public ReactiveProperty<TabContentViewModelBase> SelectedTab { get; } = new ReactiveProperty<TabContentViewModelBase>();
        public ReactiveProperty<int> SelectedTabIndex { get; }

        public ReactiveCommand OpenSettingsCommand { get; }
        public ReactiveCommand<TabContentViewModelBase> CloseTabCommand { get; }
        public ReactiveCommand PrevTabCommand { get; }
        public ReactiveCommand NextTabCommand { get; }

        public MainWindowViewModel(MainWindowModel model)
        {
            this.model = model;
            NewTootBoxViewModel = new NewTootBoxViewModel(this.model.InReplyTo, this.model.Client);
            TabViewModels = this.model.Tabs.ToReadOnlyReactiveCollection(p => TabContentViewModelBase.FromParam(p, this.model.InReplyTo, this.model.Tabs, this.model.Client));
            SelectedTabIndex = this.model.Tabs.SelectedIndex;

            OpenSettingsCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    var w = new SettingsWindow(new SettingsViewModel(this.model.Tabs));
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
