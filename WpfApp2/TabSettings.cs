using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    public class TabParameters
    {
        public string Name { get; set; }
        public TimelineType Type { get; set; }
        public bool StreamingOnStartup { get; set; }
    }
}
