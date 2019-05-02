using Mastonet;
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
    public abstract class TabContentViewModelBase
    {
        private IReactiveProperty<Status> inReplyTo;

        public ReactiveProperty<string> Name { get; }

        public TabContentViewModelBase(TabParameters param, IReactiveProperty<Status> inReplyTo)
        {
            Name = new ReactiveProperty<string>(param.Name);
            this.inReplyTo = inReplyTo;
        }

        public static TabContentViewModelBase FromParam(TabParameters param, IReactiveProperty<Status> inReplyTo, TabsModel tabs, IMastodonClient client)
        {
            switch (param)
            {
                case AccountTabParameters aparam:
                    return new AccountTabViewModel(aparam, inReplyTo, tabs, client);
                case TimelineTabParameters tparam:
                    return new TimelineViewModel(tparam, inReplyTo, tabs, client);
                case NotificationTabParameters nparam:
                    return new NotificationsViewModel(nparam, client);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
