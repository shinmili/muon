using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2
{
    class TimelineModelBase
    {
        private MastodonClient client;
        private ObservableCollection<Status> statuses;
        private TimelineStreaming streaming;
        private long? sinceId;

        public ReactiveProperty<bool> IsStreaming = new ReactiveProperty<bool>(false);
        public ReadOnlyReactiveCollection<Status> Statuses { get; }

        private readonly Func<MastodonClient, long?, long?, int?, Task<MastodonList<Status>>> GetTimeline;
        private readonly Func<MastodonClient, TimelineStreaming> GetStreaming;

        public TimelineModelBase(
            Func<MastodonClient, long?, long?, int?, Task<MastodonList<Status>>> getTimeline,
            Func<MastodonClient, TimelineStreaming> getStreaming)
        {
            GetTimeline = getTimeline;
            GetStreaming = getStreaming;
            client = new MastodonClient(Properties.Settings.Default.AppRegistration, Properties.Settings.Default.Auth);
            statuses = new ObservableCollection<Status>();
            streaming = GetStreaming(client);
            if (streaming != null)
            {
                streaming.OnUpdate += Streaming_OnUpdate;
                streaming.OnDelete += Streaming_OnDelete;
            }
            Statuses = statuses.ToReadOnlyReactiveCollection();
        }

        private void Streaming_OnDelete(object sender, StreamDeleteEventArgs e)
        {
            int? index = statuses.Select((s, i) => new { s, i })
                .FirstOrDefault(x => x.s.Id == e.StatusId)
                ?.i;
            if (index.HasValue) statuses.RemoveAt(index.Value);
        }

        private void Streaming_OnUpdate(object sender, StreamUpdateEventArgs e)
        {
            addStatus(e.Status);
        }

        public async Task StartStreamingAsync()
        {
            if (IsStreaming.Value || streaming == null) return;
            IsStreaming.Value = true;
            try { await streaming.Start(); }
            catch (TaskCanceledException) { }
        }

        public void StopStreaming()
        {
            if (!IsStreaming.Value || streaming == null) return;
            streaming.Stop();
            IsStreaming.Value = false;
        }

        public async Task ToggleStreamingAsync()
        {
            if (IsStreaming.Value)
            {
                StopStreaming();
            }
            else
            {
                await StartStreamingAsync();
            }
        }

        public async Task ReloadAsync()
        {
            var newStatuses = await GetTimeline(client, null, sinceId, null);
            newStatuses.Reverse();
            newStatuses.ForEach(addStatus);
        }

        public async Task<Status> FavouriteAsync(long id) => await client.Favourite(id);
        public async Task<Status> ReblogAsync(long id) => await client.Reblog(id);

        private void addStatus(Status status)
        {
            statuses.Add(status);
            sinceId = sinceId > status.Id ? sinceId : status.Id;
        }
    }

    class HomeTimelineModel : TimelineModelBase
    {
        public HomeTimelineModel() : base(
            (client, maxId, sinceId, limit) => client.GetHomeTimeline(maxId, sinceId, limit),
            client => client.GetUserStreaming())
        { }
    }

    class LocalTimelineModel : TimelineModelBase
    {
        public LocalTimelineModel() : base(
            (client, maxId, sinceId, limit) => client.GetPublicTimeline(maxId, sinceId, limit, true),
            client => null)
        { }
    }

    class FederatedTimelineModel : TimelineModelBase
    {
        public FederatedTimelineModel() : base(
            (client, maxId, sinceId, limit) => client.GetPublicTimeline(maxId, sinceId, limit),
            client => client.GetPublicStreaming())
        { }
    }
}
