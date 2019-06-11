using Muon.Properties;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Muon.Model
{
    public class TabsModel : ObservableCollection<TabParameters>
    {
        public ReactiveProperty<int> SelectedIndex { get; } = new ReactiveProperty<int>();

        [NonSerialized]
        private readonly Settings settings;

        internal TabsModel(Settings settings) : base(settings.Tabs)
        {
            this.settings = settings;
            CollectionChanged += (o, e) => Save();
        }

        public void Save()
        {
            settings.Reload();
            settings.Tabs = this.ToList();
            settings.Save();
        }

        public void SwitchToNextTab() => SelectedIndex.Value = (SelectedIndex.Value + 1) % Count;
        public void SwitchToPrevTab() => SelectedIndex.Value = (SelectedIndex.Value + Count - 1) % Count;
        public void OpenIfNotPresent(TabParameters p)
        {
            if (!this.Any(x => x.Equals(p))) { Add(p); }
        }
        public void SwitchToOrOpen(TabParameters p)
        {
            int? i = this
                .Select((Item, Index) => new { Item, Index })
                .FirstOrDefault(x => x.Item.Equals(p))
                ?.Index;
            if (i.HasValue)
            {
                SelectedIndex.Value = i.Value;
            }
            else
            {
                Add(p);
                SelectedIndex.Value = Count - 1;
            }
        }
        public void CloseTab(TabParameters p)
        {
            int? i = this
                .Select((Item, Index) => new { Item, Index })
                .FirstOrDefault(x => x.Item.Equals(p))
                ?.Index;
            if (i.HasValue) { CloseTab(i.Value); }
        }
        public void CloseTab(int index) => RemoveAt(index);
    }
}
