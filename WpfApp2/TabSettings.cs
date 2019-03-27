using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    public class TabParametersCollection : ObservableCollection<TabParameters>
    {
        public TabParametersCollection() { }
        public TabParametersCollection(IEnumerable<TabParameters> xs) : base(xs) { }
    }

    public class TabParameters
    {
        public TimelineType Type { get; set; }
        public bool StreamingOnStartup { get; set; }
    }
}
