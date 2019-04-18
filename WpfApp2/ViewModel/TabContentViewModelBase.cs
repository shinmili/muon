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
        public ReactiveProperty<string> Name { get; }

        public TabContentViewModelBase(TabParameters param)
        {
            Name = new ReactiveProperty<string>(param.Name);
        }

        public static TabContentViewModelBase FromParam(TabParameters param)
        {
            switch (param)
            {
                case AccountTabParameters aparam:
                    return new AccountTabViewModel(aparam);
                case TimelineTabParameters tparam:
                    return new TimelineViewModel(tparam);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
