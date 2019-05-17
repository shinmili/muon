using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Muon.Model;

namespace Muon.ViewModel
{
    public class StatusesViewModel
    {
        private TimelineModelBase model;
        private IReactiveProperty<Status> inReplyTo;
        private TabsModel tabs;

        public ReadOnlyReactiveCollection<StatusViewModel> Statuses { get; }
        public ReadOnlyReactiveProperty<bool> IsStreaming { get; }
        public bool IsStreamingAvailable { get; }
        public ReactiveProperty<StatusViewModel> SelectedStatus { get; } = new ReactiveProperty<StatusViewModel>();

        public AsyncReactiveCommand ReloadCommand { get; }
        public AsyncReactiveCommand ReloadOlderCommand { get; }
        public ReactiveCommand ToggleStreamingCommand { get; }
        public ReactiveCommand OpenCommand { get; }
        public ReactiveCommand ReplyCommand { get; }
        public AsyncReactiveCommand FavouriteCommand { get; }
        public AsyncReactiveCommand ReblogCommand { get; }
        public AsyncReactiveCommand DeleteCommand { get; }
        public ReactiveCommand OpenAccountTabCommand { get; }

        public StatusesViewModel(TimelineModelBase model, IReactiveProperty<Status> inReplyTo, TabsModel tabs, bool streamingOnStartup = false)
        {
            this.model = model;
            this.inReplyTo = inReplyTo;
            this.tabs = tabs;
            IsStreaming = this.model.StreamingStarted.ToReadOnlyReactiveProperty();
            IsStreamingAvailable = this.model.IsStreamingAvailable;
            Statuses = this.model.ToReadOnlyReactiveCollection(s => new StatusViewModel(s));

            ReloadCommand = new AsyncReactiveCommand()
                .WithSubscribe(() => this.model.FetchPreviousAsync());
            ReloadOlderCommand = new AsyncReactiveCommand()
                .WithSubscribe(() => this.model.FetchNextAsync());
            ToggleStreamingCommand = Observable.Repeat(IsStreamingAvailable, 1).ToReactiveCommand()
                .WithSubscribe(() => this.model.StreamingStarting.Value = !IsStreaming.Value);

            var IsStatusSelected = SelectedStatus.Select(x => x != null);
            OpenCommand = IsStatusSelected.ToReactiveCommand()
                .WithSubscribe(() => Process.Start(SelectedStatus.Value.Status.Url ?? SelectedStatus.Value.Status.Reblog.Url));
            FavouriteCommand = IsStatusSelected.ToAsyncReactiveCommand()
                .WithSubscribe(() => this.model.FavouriteAsync(SelectedStatus.Value.Status.Id));
            ReblogCommand = IsStatusSelected.ToAsyncReactiveCommand()
                .WithSubscribe(() => this.model.ReblogAsync(SelectedStatus.Value.Status.Id));
            ReplyCommand = IsStatusSelected.ToReactiveCommand()
                .WithSubscribe(() => this.inReplyTo.Value = SelectedStatus.Value.Status);
            DeleteCommand = IsStatusSelected.ToAsyncReactiveCommand()
                .WithSubscribe(() => this.model.DeleteAsync(SelectedStatus.Value.Status.Id));

            OpenAccountTabCommand = IsStatusSelected.ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    Account account = SelectedStatus.Value.OriginalStatus.Account;
                    this.tabs.SwitchToOrOpen(new AccountTabParameters() { Name = $"user: {account.AccountName}", Id = account.Id });
                });

            this.model.StreamingStarting.Value = streamingOnStartup;
            ReloadCommand.Execute();
        }
    }
}
