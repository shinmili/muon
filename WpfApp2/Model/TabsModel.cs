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

        private void InitializeAutoSave() => CollectionChanged += (o, e) => Save();
    }
}
