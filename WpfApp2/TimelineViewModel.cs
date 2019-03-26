using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
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
            Statuses = model.Statuses.ToReadOnlyReactiveCollection(s => new StatusViewModel(s));

            ReloadCommand = new AsyncReactiveCommand()
                .WithSubscribe(async () => await model.ReloadAsync());
            ToggleStreamingCommand = new ReactiveCommand()
                .WithSubscribe(async () => await model.ToggleStreamingAsync());

            var IsStatusSelected = SelectedItem.Select(x => x != null);
            OpenCommand = IsStatusSelected.ToReactiveCommand<StatusViewModel>()
                .WithSubscribe(p => Process.Start(p.Status.Url ?? p.Status.Reblog.Url));
            FavouriteCommand = IsStatusSelected.ToAsyncReactiveCommand<StatusViewModel>()
                .WithSubscribe(async p => await model.FavouriteAsync(p.Status.Id));
            ReblogCommand = IsStatusSelected.ToAsyncReactiveCommand<StatusViewModel>()
                .WithSubscribe(async p => await model.ReblogAsync(p.Status.Id));
            MentionCommand = IsStatusSelected.ToReactiveCommand<StatusViewModel>()
                .WithSubscribe(p => InReplyTo.Value = p);
        }

        public ReadOnlyReactiveCollection<StatusViewModel> Statuses { get; }
        public ReadOnlyReactiveProperty<bool> IsStreaming { get; }
        public ReactiveProperty<StatusViewModel> InReplyTo { get; } = new ReactiveProperty<StatusViewModel>();
        public ReactiveProperty<StatusViewModel> SelectedItem { get; } = new ReactiveProperty<StatusViewModel>();

        public AsyncReactiveCommand ReloadCommand { get; }
        public ReactiveCommand ToggleStreamingCommand { get; }
        public ReactiveCommand<StatusViewModel> OpenCommand { get; }
        public ReactiveCommand<StatusViewModel> MentionCommand { get; }
        public AsyncReactiveCommand<StatusViewModel> FavouriteCommand { get; }
        public AsyncReactiveCommand<StatusViewModel> ReblogCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
