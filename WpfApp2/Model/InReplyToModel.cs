using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Model
{
    class InReplyToModel
    {
        private static InReplyToModel instance = new InReplyToModel();
        public static InReplyToModel Instance => instance;
        public ReactiveProperty<Status> InReplyTo { get; } = new ReactiveProperty<Status>();
        private InReplyToModel()
        {
            InReplyTo.Subscribe(x => Console.WriteLine(x?.Content));
        }
    }
}
