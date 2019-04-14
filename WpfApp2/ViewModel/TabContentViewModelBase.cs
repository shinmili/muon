using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return new TimelineViewModel((TimelineTabParameters)param);
        }
    }
}
