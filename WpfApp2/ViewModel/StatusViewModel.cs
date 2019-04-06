using HtmlAgilityPack;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace WpfApp2
{
    class StatusViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Status Status { get; }
        public Status OriginalStatus { get; }
        public string StaticAvatarUrl { get; }
        public string DisplayName { get; }
        public ReactiveProperty<bool> Deleted { get; }

        public StatusViewModel(Status s)
        {
            Status = s;
            OriginalStatus = s.Reblog ?? s;

            StaticAvatarUrl = OriginalStatus.Account.StaticAvatarUrl;
            DisplayName = OriginalStatus.Account.DisplayName + (s.Reblog == null ? "" : $"(RT:{s.Account.AccountName})");
        }
    }
}
