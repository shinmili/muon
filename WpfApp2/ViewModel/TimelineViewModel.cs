using Reactive.Bindings;
using System.ComponentModel;
using WpfApp2.Model;

namespace WpfApp2.ViewModel
{
    class TimelineViewModel : TabContentViewModelBase, INotifyPropertyChanged
    {
        public StatusesViewModel Statuses { get; }
        public ReadOnlyReactiveProperty<bool> IsStreaming => Statuses.IsStreaming;
        public AsyncReactiveCommand ReloadCommand => Statuses.ReloadCommand;
        public AsyncReactiveCommand ReloadOlderCommand => Statuses.ReloadOlderCommand;
        public ReactiveCommand ToggleStreamingCommand => Statuses.ToggleStreamingCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public TimelineViewModel(TimelineTabParameters param) : base(param)
        {
            switch (param.Type)
            {
                case TimelineType.Home:
                    Statuses = new StatusesViewModel(new HomeTimelineModel(), param.StreamingOnStartup);
                    break;
                case TimelineType.Local:
                    Statuses = new StatusesViewModel(new LocalTimelineModel(), param.StreamingOnStartup);
                    break;
                case TimelineType.Federated:
                    Statuses = new StatusesViewModel(new FederatedTimelineModel(), param.StreamingOnStartup);
                    break;
            }
        }
    }
}
