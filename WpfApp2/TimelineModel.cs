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
        private SettingsModel settings;
        private ReactiveCollection<Status> statuses;
        private TimelineStreaming streaming;
        public ReactiveProperty<bool> IsStreaming = new ReactiveProperty<bool>(false);
        private long? sinceId;

        public ReadOnlyReactiveCollection<Status> Statuses { get; }

        private Func<MastodonClient, long?, long?, int?, Task<MastodonList<Status>>> GetTimeline;
        private Func<MastodonClient, TimelineStreaming> GetStreaming;

        public TimelineModelBase(Func<MastodonClient, long?, long?, int?, Task<MastodonList<Status>>> getTimeline, Func<MastodonClient, TimelineStreaming> getStreaming)
        {
            GetTimeline = getTimeline;
            GetStreaming = getStreaming;
            settings = new SettingsModel();
            client = new MastodonClient(settings.AppRegistration, settings.Auth);
            statuses = new ReactiveCollection<Status>();
            streaming = GetStreaming(client);
            streaming.OnUpdate += Streaming_OnUpdate;
            streaming.OnDelete += Streaming_OnDelete;
            Statuses = statuses.ToReadOnlyReactiveCollection();
        }

        private void Streaming_OnDelete(object sender, StreamDeleteEventArgs e)
        {
            int index = statuses.Select((s, i) => new { s, i })
                .First(x => x.s.Id == e.StatusId)
                .i;
            statuses.RemoveAt(index);
        }

        private void Streaming_OnUpdate(object sender, StreamUpdateEventArgs e)
        {
            addStatus(e.Status);
        }

        public async Task StartStreamingAsync()
        {
            if (IsStreaming.Value) return;
            IsStreaming.Value = true;
            try { await streaming.Start(); }
            catch (TaskCanceledException) { }
        }

        public void StopStreaming()
        {
            if (!IsStreaming.Value) return;
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

        private void addStatus(Status status)
        {
            statuses.Add(status);
            sinceId = status.Id;
        }
    }

    class HomeTimelineModel : TimelineModelBase
    {
        public HomeTimelineModel() : base(
            (client, maxId, sinceId, limit) => client.GetHomeTimeline(maxId, sinceId, limit),
            client => client.GetUserStreaming())
        { }
    }
}
