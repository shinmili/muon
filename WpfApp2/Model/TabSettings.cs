using Reactive.Bindings;
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
    [XmlInclude(typeof(AccountTabParameters))]
    [XmlInclude(typeof(NotificationTabParameters))]
    public class TabParameters
    {
        public string Name { get; set; }
    }

    public class TimelineTabParameters : TabParameters, IStreamingTabParameters
    {
        public TimelineType Type { get; set; }
        public bool StreamingOnStartup { get; set; }
    }

    public class AccountTabParameters : TabParameters
    {
        public long Id { get; set; }
    }

    public class NotificationTabParameters : TabParameters, IStreamingTabParameters
    {
        public bool StreamingOnStartup { get; set; }
    }

    interface IStreamingTabParameters
    {
        bool StreamingOnStartup { get; set; }
    }

    public class TabSettingsModel : ObservableCollection<TabParameters>
    {
        private static TabSettingsModel defaultInstance;
        public static TabSettingsModel Default => defaultInstance ?? (defaultInstance = new TabSettingsModel());

        public ReactiveProperty<int> SelectedIndex { get; } = new ReactiveProperty<int>();

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
