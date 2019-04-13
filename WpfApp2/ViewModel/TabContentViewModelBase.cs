using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.ViewModel
{
    abstract class TabContentViewModelBase
    {
        public static TabContentViewModelBase FromParam(TabParameters param)
        {
            return new TimelineViewModel((TimelineTabParameters)param);
        }
    }
}
