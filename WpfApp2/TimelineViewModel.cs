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
        private TimelineModel model;

        public TimelineViewModel()
        {
            model = new TimelineModel();
            IsStreaming = model.IsStreaming.ToReadOnlyReactiveProperty();
            ReloadCommand = new DelegateCommand { ExecuteHandler = async _ => await model.ReloadAsync() };
            ToggleStreamingCommand = new DelegateCommand { ExecuteHandler = async _ => await model.ToggleStreamingAsync() };
            OpenCommand = new DelegateCommand { ExecuteHandler = p => Process.Start(((StatusViewModel)p).Status.Url ?? ((StatusViewModel)p).Status.Reblog.Url) };
        }

        public ReadOnlyReactiveCollection<StatusViewModel> Statuses
            => model.Statuses.ToReadOnlyReactiveCollection(s => new StatusViewModel(s));
        public ReadOnlyReactiveProperty<bool> IsStreaming { get; }

        public DelegateCommand ReloadCommand { get; }
        public DelegateCommand ToggleStreamingCommand { get; }
        public DelegateCommand OpenCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
