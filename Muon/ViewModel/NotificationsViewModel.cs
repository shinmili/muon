using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Muon.Model;

namespace Muon.ViewModel
{
    public class NotificationsViewModel : TabContentViewModelBase
    {
        public event EventHandler<Notification> OnNotification
        {
            add { model.OnNotification += value; }
            remove { model.OnNotification -= value; }
        }

        private NotificationsModel model;

        public ReadOnlyObservableCollection<Notification> Notifications { get; }
        public ReadOnlyReactiveProperty<bool> IsStreaming { get; }

        public AsyncReactiveCommand ReloadCommand { get; }
        public AsyncReactiveCommand ReloadOlderCommand { get; }
        public ReactiveCommand ToggleStreamingCommand { get; }

        public NotificationsViewModel(NotificationTabParameters param, IMastodonClient client) : base(param, null)
        {
            model = new NotificationsModel(client);
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
