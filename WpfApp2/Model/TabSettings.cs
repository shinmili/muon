using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WpfApp2
{
    [XmlInclude(typeof(TimelineTabParameters))]
    public class TabParameters
    {
        public string Name { get; set; }
    }

    public class TimelineTabParameters : TabParameters
    {
        public TimelineType Type { get; set; }
        public bool StreamingOnStartup { get; set; }
    }
}
