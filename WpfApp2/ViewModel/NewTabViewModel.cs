using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Model;

namespace WpfApp2.ViewModel
{
    class NewTabViewModel
    {
        public static IEnumerable<TimelineType> Types { get; } = new List<TimelineType> { TimelineType.Home, TimelineType.Local, TimelineType.Federated };
        public ReactiveProperty<TimelineType?> SelectedType { get; } = new ReactiveProperty<TimelineType?>();
        public ReactiveCommand AddCommand { get; }
        public TabSettingsModel Tabs { get; set; }
        public NewTabViewModel()
        {
            AddCommand = SelectedType
                .Select(x => x != null)
                .ToReactiveCommand()
                .WithSubscribe(() => Tabs.Add(new TimelineTabParameters() { Type = SelectedType.Value.Value, Name = SelectedType.Value.ToString(), StreamingOnStartup = false }));
        }
    }
}
