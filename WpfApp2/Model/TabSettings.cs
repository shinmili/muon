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
    public class TabParameters : IEquatable<TabParameters>
    {
        public string Name { get; set; }

        public bool Equals(TabParameters other)
        {
            if (this.GetType() != other.GetType()) return false;
            switch (this) {
                case TimelineTabParameters _this:
                    return _this.Equals((TimelineTabParameters)other);
                case AccountTabParameters _this:
                    return _this.Equals((AccountTabParameters)other);
                case NotificationTabParameters _this:
                    return _this.Equals((NotificationTabParameters)other);
                default:
                    return false;
            }
        }
    }

    public class TimelineTabParameters : TabParameters, IStreamingTabParameters, IEquatable<TimelineTabParameters>
    {
        public TimelineType Type { get; set; }
        public bool StreamingOnStartup { get; set; }
        public bool Equals(TimelineTabParameters other) => Type == other.Type;
    }

    public class AccountTabParameters : TabParameters, IEquatable<AccountTabParameters>
    {
        public long Id { get; set; }
        public bool Equals(AccountTabParameters other) => Id == other.Id;
    }

    public class NotificationTabParameters : TabParameters, IStreamingTabParameters, IEquatable<NotificationTabParameters>
    {
        public bool StreamingOnStartup { get; set; }
        public bool Equals(NotificationTabParameters other) => true;
    }

    interface IStreamingTabParameters
    {
        bool StreamingOnStartup { get; set; }
    }
}
