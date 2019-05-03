using Reactive.Bindings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WpfApp2.Model
{
    public class TabsModel : ObservableCollection<TabParameters>
    {
        public ReactiveProperty<int> SelectedIndex { get; } = new ReactiveProperty<int>();

        public TabsModel() => InitializeAutoSave();
        public TabsModel(List<TabParameters> list) : base(list) => InitializeAutoSave();
        public TabsModel(IEnumerable<TabParameters> collection) : base(collection) => InitializeAutoSave();

        public void Save()
        {
            Properties.Settings.Default.Reload();
            Properties.Settings.Default.Tabs = this.ToList();
            Properties.Settings.Default.Save();
        }

        public void SwitchToNextTab() => SelectedIndex.Value = (SelectedIndex.Value + 1) % Count;
        public void SwitchToPrevTab() => SelectedIndex.Value = (SelectedIndex.Value + Count - 1) % Count;
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

        private void InitializeAutoSave() => CollectionChanged += (o, e) => Save();
    }
}
