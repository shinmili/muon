using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muon.Model
{
    public class MainWindowModel : INotifyPropertyChanged
    {
        public ReactiveProperty<Status> InReplyTo { get; }
        public MastodonClient Client { get; }
        public TabsModel Tabs { get; }

        public MainWindowModel(ReactiveProperty<Status> irt, MastodonClient cli, TabsModel tabs)
        {
            InReplyTo = irt;
            Client = cli;
            Tabs = tabs;
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
