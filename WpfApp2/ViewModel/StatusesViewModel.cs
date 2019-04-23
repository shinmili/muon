using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using WpfApp2.Model;

namespace WpfApp2.ViewModel
{
    class StatusesViewModel
    {
        private TimelineModelBase model;
        private InReplyToModel inReplyToModel = InReplyToModel.Instance;
        private TabSettingsModel tabs = TabSettingsModel.Default;

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

        public StatusesViewModel(TimelineModelBase model, bool streamingOnStartup = false)
        {
            this.model = model;
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
                .WithSubscribe(() => inReplyToModel.InReplyTo.Value = SelectedStatus.Value.Status);
            DeleteCommand = IsStatusSelected.ToAsyncReactiveCommand()
                .WithSubscribe(() => this.model.DeleteAsync(SelectedStatus.Value.Status.Id));

            OpenAccountTabCommand = IsStatusSelected.ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    long id = SelectedStatus.Value.OriginalStatus.Account.Id;
                    int? i = tabs.Select((Value, Index) => new { Value, Index }).FirstOrDefault(x => (x.Value as AccountTabParameters)?.Id == id)?.Index;
                    if (i.HasValue)
                    {
                        tabs.SelectedIndex.Value = i.Value;
                    }
                    else
                    {
                        tabs.Add(new AccountTabParameters() { Id = id, Name = SelectedStatus.Value.OriginalStatus.Account.AccountName });
                        tabs.SelectedIndex.Value = tabs.Count() - 1;
                    }
                });

            this.model.StreamingStarting.Value = streamingOnStartup;
            ReloadCommand.Execute();
        }
    }
}
