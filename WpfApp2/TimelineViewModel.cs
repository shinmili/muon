using Mastonet.Entities;
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
    class TimelineViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TimelineModel timeline;

        public TimelineViewModel()
        {
            timeline = new TimelineModel();
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ReadOnlyObservableCollection<Status> Statuses { get => timeline.Statuses; }

        private DelegateCommand reloadCommand;
        public DelegateCommand ReloadCommand
        {
            get => reloadCommand ?? (reloadCommand = new DelegateCommand
            {
                ExecuteHandler = async _ => await timeline.ReloadAsync(),
                CanExecuteHandler = null
            });
        }

        private DelegateCommand startStreamingCommand;
        public DelegateCommand StartStreamingCommand
        {
            get => startStreamingCommand ?? (startStreamingCommand = new DelegateCommand
            {
                ExecuteHandler = async _ => await timeline.ToggleStreamingAsync(),
                CanExecuteHandler = null
            });
        }

    }
}
