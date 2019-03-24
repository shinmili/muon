﻿using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    class TimelineViewModel : INotifyPropertyChanged
    {
        private TimelineModelBase model;

        public TimelineViewModel(TimelineModelBase model)
        {
            this.model = model;
            IsStreaming = model.IsStreaming.ToReadOnlyReactiveProperty();
            ReloadCommand = new AsyncReactiveCommand()
                .WithSubscribe(async () => await model.ReloadAsync());
            ToggleStreamingCommand = new AsyncReactiveCommand()
                .WithSubscribe(async () => await model.ToggleStreamingAsync());
            OpenCommand = new ReactiveCommand<StatusViewModel>()
                .WithSubscribe(p => Process.Start(p.Status.Url ?? p.Status.Reblog.Url));
        }

        public ReadOnlyReactiveCollection<StatusViewModel> Statuses
            => model.Statuses.ToReadOnlyReactiveCollection(s => new StatusViewModel(s));
        public ReadOnlyReactiveProperty<bool> IsStreaming { get; }

        public AsyncReactiveCommand ReloadCommand { get; }
        public AsyncReactiveCommand ToggleStreamingCommand { get; }
        public ReactiveCommand<StatusViewModel> OpenCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
