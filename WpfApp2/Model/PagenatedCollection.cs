using Mastonet;
using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muon.Model
{
    public abstract class PagenatedCollection<T> : ObservableCollection<T>
    {
        private long? sinceId;
        private long? maxId;

        public int? Limit { get; set; }

        public ArrayOptions Previous => new ArrayOptions() { SinceId = sinceId, Limit = Limit };
        public ArrayOptions Next => new ArrayOptions() { MaxId = maxId, Limit = Limit };

        public PagenatedCollection() : base() { }

        protected void UpdateSinceId(long id)
        {
            if (!sinceId.HasValue || sinceId < id)
            {
                sinceId = id;
            }
        }

        protected void UpdateMaxId(long id)
        {
            if (!maxId.HasValue || maxId > id)
            {
                maxId = id;
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            long id = (long)typeof(T).GetProperty("Id").GetValue(item);
            UpdateSinceId(id);
            UpdateMaxId(id);
        }

        public void AddMastodonList(MastodonList<T> items)
        {
            foreach (T item in items) { InsertItem(this.Count(), item); }
            if (items.PreviousPageSinceId.HasValue) { UpdateSinceId(items.PreviousPageSinceId.Value); }
            if (items.NextPageMaxId.HasValue) { UpdateMaxId(items.NextPageMaxId.Value); }
        }

        protected abstract Task<MastodonList<T>> FetchAsync(ArrayOptions options);

        public async Task FetchPreviousAsync() => AddMastodonList(await FetchAsync(Previous));
        public async Task FetchNextAsync() => AddMastodonList(await FetchAsync(Next));
    }
}
