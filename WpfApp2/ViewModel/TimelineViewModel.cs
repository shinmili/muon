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
using WpfApp2.Model;
using WpfApp2.ViewModel;

namespace WpfApp2
{
    class TimelineViewModel : TabContentViewModelBase, INotifyPropertyChanged
    {
        private TimelineModelBase model;
        private InReplyToModel inReplyToModel = InReplyToModel.Instance;

        public TimelineViewModel(TimelineTabParameters param) : base(param)
        {
            Type = param.Type;
            switch (Type)
            {
                case TimelineType.Home:
                    model = new HomeTimelineModel();
                    break;
                case TimelineType.Local:
                    model = new LocalTimelineModel();
                    break;
                case TimelineType.Federated:
                    model = new FederatedTimelineModel();
                    break;
            }
            IsStreaming = model.StreamingStarted;
            Statuses = model.Statuses.ToReadOnlyReactiveCollection(s => new StatusViewModel(s));

            ReloadCommand = new AsyncReactiveCommand()
                .WithSubscribe(() => model.ReloadAsync());
            ToggleStreamingCommand = new ReactiveCommand()
                .WithSubscribe(() => model.StreamingStarting.Value = !IsStreaming.Value);

            var IsStatusSelected = SelectedItem.Select(x => x != null);
            OpenCommand = IsStatusSelected.ToReactiveCommand<StatusViewModel>()
                .WithSubscribe(p => Process.Start(p.Status.Url ?? p.Status.Reblog.Url));
            FavouriteCommand = IsStatusSelected.ToAsyncReactiveCommand<StatusViewModel>()
                .WithSubscribe(p => model.FavouriteAsync(p.Status.Id));
            ReblogCommand = IsStatusSelected.ToAsyncReactiveCommand<StatusViewModel>()
                .WithSubscribe(p => model.ReblogAsync(p.Status.Id));
            ReplyCommand = IsStatusSelected.ToReactiveCommand<StatusViewModel>()
                .WithSubscribe(p => inReplyToModel.InReplyTo.Value = p.Status);
            DeleteCommand = IsStatusSelected.ToAsyncReactiveCommand<StatusViewModel>()
                .WithSubscribe(p => model.DeleteAsync(p.Status.Id));

            model.StreamingStarting.Value = param.StreamingOnStartup;
            ReloadCommand.Execute();
        }

        public TimelineType Type { get; }
        public ReadOnlyReactiveCollection<StatusViewModel> Statuses { get; }
        public ReadOnlyReactiveProperty<bool> IsStreaming { get; }
        public ReactiveProperty<StatusViewModel> SelectedItem { get; } = new ReactiveProperty<StatusViewModel>();

        public AsyncReactiveCommand ReloadCommand { get; }
        public ReactiveCommand ToggleStreamingCommand { get; }
        public ReactiveCommand<StatusViewModel> OpenCommand { get; }
        public ReactiveCommand<StatusViewModel> ReplyCommand { get; }
        public AsyncReactiveCommand<StatusViewModel> FavouriteCommand { get; }
        public AsyncReactiveCommand<StatusViewModel> ReblogCommand { get; }
        public AsyncReactiveCommand<StatusViewModel> DeleteCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum TimelineType { Home, Local, Federated }

}
