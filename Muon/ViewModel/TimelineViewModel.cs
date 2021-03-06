﻿using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System.ComponentModel;
using Muon.Model;

namespace Muon.ViewModel
{
    public class TimelineViewModel : TabContentViewModelBase, INotifyPropertyChanged
    {
        public StatusesViewModel Statuses { get; }
        public ReadOnlyReactiveProperty<bool> IsStreaming => Statuses.IsStreaming;
        public AsyncReactiveCommand ReloadCommand => Statuses.ReloadCommand;
        public AsyncReactiveCommand ReloadOlderCommand => Statuses.ReloadOlderCommand;
        public ReactiveCommand ToggleStreamingCommand => Statuses.ToggleStreamingCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public TimelineViewModel(TimelineTabParameters param, IReactiveProperty<Status> inReplyTo, TabsModel tabs, IMastodonClient client) : base(param, inReplyTo)
        {
            switch (param.Type)
            {
                case TimelineType.Home:
                    Statuses = new StatusesViewModel(new HomeTimelineModel(client), inReplyTo, tabs, param.StreamingOnStartup);
                    break;
                case TimelineType.Local:
                    Statuses = new StatusesViewModel(new LocalTimelineModel(client), inReplyTo, tabs, param.StreamingOnStartup);
                    break;
                case TimelineType.Federated:
                    Statuses = new StatusesViewModel(new FederatedTimelineModel(client), inReplyTo, tabs, param.StreamingOnStartup);
                    break;
            }
        }
    }
}
