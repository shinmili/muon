using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    class TimelineViewModel
    {
        private TimelineModel model;

        public TimelineViewModel()
        {
            model = new TimelineModel();
            IsStreaming = model.IsStreaming.ToReadOnlyReactiveProperty();
            ReloadCommand = new DelegateCommand { ExecuteHandler = async _ => await model.ReloadAsync() };
            ToggleStreamingCommand = new DelegateCommand { ExecuteHandler = async _ => await model.ToggleStreamingAsync() };
        }

        public ReadOnlyReactiveCollection<StatusViewModel> Statuses
            => model.Statuses.ToReadOnlyReactiveCollection(s => new StatusViewModel(s));
        public ReadOnlyReactiveProperty<bool> IsStreaming { get; }

        public DelegateCommand ReloadCommand { get; }
        public DelegateCommand ToggleStreamingCommand { get; }

    }
}
