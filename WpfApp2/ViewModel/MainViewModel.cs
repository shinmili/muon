using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.ViewModel;

namespace WpfApp2
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ReadOnlyReactiveCollection<TabContentViewModelBase> TimelineViewModels { get; }

        public ReactiveCommand OpenSettingsCommand { get; }

        public MainViewModel()
        {
            TimelineViewModels = Properties.Settings.Default.Tabs.ToReadOnlyReactiveCollection(p => TabContentViewModelBase.FromParam(p));

            OpenSettingsCommand = new ReactiveCommand()
                .WithSubscribe(() => new SettingsWindow().ShowDialog());
        }
    }
}
