﻿using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Model;

namespace WpfApp2.ViewModel
{
    class NotificationsViewModel : TabContentViewModelBase
    {
        private NotificationsModel model = new NotificationsModel();

        public ReadOnlyObservableCollection<Notification> Notifications { get; }
        public ReadOnlyReactiveProperty<bool> IsStreaming { get; }

        public AsyncReactiveCommand ReloadCommand { get; }
        public AsyncReactiveCommand ReloadOlderCommand { get; }
        public ReactiveCommand ToggleStreamingCommand { get; }

        public NotificationsViewModel(NotificationTabParameters param) : base(param)
        {
            Notifications = new ReadOnlyObservableCollection<Notification>(model);
            IsStreaming = model.StreamingStarted;
            ReloadCommand = new AsyncReactiveCommand().WithSubscribe(() => model.FetchPreviousAsync());
            ReloadOlderCommand = new AsyncReactiveCommand().WithSubscribe(() => model.FetchNextAsync());
            ToggleStreamingCommand = new ReactiveCommand().WithSubscribe(() => model.StreamingStarting.Value = !IsStreaming.Value);

            model.StreamingStarting.Value = param.StreamingOnStartup;
            ReloadCommand.Execute();
        }
    }
}
