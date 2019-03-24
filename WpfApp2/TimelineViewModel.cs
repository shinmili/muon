using Mastonet.Entities;
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
            ToggleStreamingCommand = new ReactiveCommand()
                .WithSubscribe(async () => await model.ToggleStreamingAsync());
            OpenCommand = new ReactiveCommand<StatusViewModel>()
                .WithSubscribe(p => Process.Start(p.Status.Url ?? p.Status.Reblog.Url));
            FavouriteCommand = new AsyncReactiveCommand<StatusViewModel>()
                .WithSubscribe(async p => await model.FavouriteAsync(p.Status.Id));
            ReblogCommand = new AsyncReactiveCommand<StatusViewModel>()
                .WithSubscribe(async p => await model.ReblogAsync(p.Status.Id));
        }

        public ReadOnlyReactiveCollection<StatusViewModel> Statuses
            => model.Statuses.ToReadOnlyReactiveCollection(s => new StatusViewModel(s));
        public ReadOnlyReactiveProperty<bool> IsStreaming { get; }

        public AsyncReactiveCommand ReloadCommand { get; }
        public ReactiveCommand ToggleStreamingCommand { get; }
        public ReactiveCommand<StatusViewModel> OpenCommand { get; }
        public AsyncReactiveCommand<StatusViewModel> FavouriteCommand { get; }
        public AsyncReactiveCommand<StatusViewModel> ReblogCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
