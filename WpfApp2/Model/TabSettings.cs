using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WpfApp2.Model
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

    public class TabSettingsModel : ObservableCollection<TabParameters>
    {
        private static TabSettingsModel defaultInstance;
        public static TabSettingsModel Default { get { return defaultInstance ?? (defaultInstance = new TabSettingsModel()); } }

        public TabSettingsModel() : base(Properties.Settings.Default.Tabs)
        {
            CollectionChanged += (o, e) =>
            {
                Properties.Settings.Default.Tabs = this.ToList();
                Properties.Settings.Default.Save();
            };
        }
    }
}
