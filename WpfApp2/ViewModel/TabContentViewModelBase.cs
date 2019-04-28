using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Model;

namespace WpfApp2.ViewModel
{
    abstract class TabContentViewModelBase
    {
        private IReactiveProperty<Status> inReplyTo;

        public ReactiveProperty<string> Name { get; }

        public TabContentViewModelBase(TabParameters param, IReactiveProperty<Status> inReplyTo)
        {
            Name = new ReactiveProperty<string>(param.Name);
            this.inReplyTo = inReplyTo;
        }

        public static TabContentViewModelBase FromParam(TabParameters param, IReactiveProperty<Status> inReplyTo)
        {
            switch (param)
            {
                case AccountTabParameters aparam:
                    return new AccountTabViewModel(aparam, inReplyTo);
                case TimelineTabParameters tparam:
                    return new TimelineViewModel(tparam, inReplyTo);
                case NotificationTabParameters nparam:
                    return new NotificationsViewModel(nparam);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
