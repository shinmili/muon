using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Model
{
    class MainModel : INotifyPropertyChanged
    {
        public ReactiveProperty<Status> InReplyTo { get; } = new ReactiveProperty<Status>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
