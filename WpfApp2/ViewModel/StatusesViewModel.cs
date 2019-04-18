using Reactive.Bindings;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
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
        public ReactiveProperty<StatusViewModel> SelectedStatus { get; } = new ReactiveProperty<StatusViewModel>();

        public AsyncReactiveCommand ReloadCommand { get; }
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
            Statuses = this.model.Statuses.ToReadOnlyReactiveCollection(s => new StatusViewModel(s));

            ReloadCommand = new AsyncReactiveCommand()
                .WithSubscribe(() => this.model.ReloadAsync());
            ToggleStreamingCommand = new ReactiveCommand()
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
                .WithSubscribe(() => tabs.Add(new AccountTabParameters() { Id = SelectedStatus.Value.OriginalStatus.Account.Id, Name = SelectedStatus.Value.OriginalStatus.Account.AccountName }));

            this.model.StreamingStarting.Value = streamingOnStartup;
            ReloadCommand.Execute();
        }
    }
}
