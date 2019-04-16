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
using WpfApp2.Model;
using WpfApp2.View;

namespace WpfApp2.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ReadOnlyReactiveCollection<TabContentViewModelBase> TabViewModels { get; }

        public ReactiveCommand OpenSettingsCommand { get; }
        public ReactiveCommand NewTabCommand { get; }
        public ReactiveCommand<TimelineViewModel> CloseTabCommand { get; }

        private TabSettingsModel tabs = TabSettingsModel.Default;

        public MainViewModel()
        {
            TabViewModels = tabs.ToReadOnlyReactiveCollection(p => TabContentViewModelBase.FromParam(p));

            OpenSettingsCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    var w = new SettingsWindow();
                    ((SettingsViewModel)w.DataContext).Tabs = tabs;
                    w.ShowDialog();
                });
            NewTabCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    var w = new NewTabWindow();
                    ((NewTabViewModel)w.DataContext).Tabs = tabs;
                    w.ShowDialog();
                });
            CloseTabCommand = new ReactiveCommand<TimelineViewModel>()
                .WithSubscribe(vm =>
                {
                    tabs.RemoveAt(TabViewModels.IndexOf(vm));
                });
        }
    }
}
