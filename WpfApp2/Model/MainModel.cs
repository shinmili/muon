﻿using Mastonet;
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
    public class MainModel : INotifyPropertyChanged
    {
        public ReactiveProperty<Status> InReplyTo { get; }
        public MastodonClient Client { get; }

        public MainModel(ReactiveProperty<Status> irt, MastodonClient cli)
        {
            InReplyTo = irt;
            Client = cli;
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
