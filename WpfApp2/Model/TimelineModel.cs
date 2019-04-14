using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2
{
    abstract class TimelineModelBase
    {
        protected MastodonClient client = new MastodonClient(Properties.Settings.Default.AppRegistration, Properties.Settings.Default.Auth);
        private ObservableCollection<Status> statuses = new ObservableCollection<Status>();
        private TimelineStreaming streaming;
        private long? sinceId;

        /// <summary>
        /// Set this property to start or stop streaming.
        /// </summary>
        public ReactiveProperty<bool> StreamingStarting { get; } = new ReactiveProperty<bool>(false);

        /// <summary>
        /// Tells if the streaming is actually started.
        /// </summary>
        public ReadOnlyReactiveProperty<bool> StreamingStarted { get; }
        private ReactiveProperty<bool> streamingStarted = new ReactiveProperty<bool>(false);

        public ReadOnlyReactiveCollection<Status> Statuses { get; }

        protected abstract Task<MastodonList<Status>> GetTimeline(ArrayOptions options);
        protected abstract TimelineStreaming GetStreaming();

        protected TimelineModelBase()
        {
            Statuses = statuses.ToReadOnlyReactiveCollection();

            StreamingStarted = streamingStarted.ToReadOnlyReactiveProperty();
            streaming = GetStreaming();
            if (streaming != null)
            {
                streaming.OnUpdate += Streaming_OnUpdate;
                streaming.OnDelete += Streaming_OnDelete;
            }
            StreamingStarting.DistinctUntilChanged().Subscribe(OnStreamingChanged);
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

        private async void OnStreamingChanged(bool b)
        {
            if (b) { await StartStreamingAsync(); }
            else { StopStreaming(); }
        }

        private async Task StartStreamingAsync()
        {
            try
            {
                streamingStarted.Value = true;
                await streaming?.Start();
            }
            catch (TaskCanceledException) { }
            finally { streamingStarted.Value = false; }
        }

        private void StopStreaming() => streaming?.Stop();

        public async Task ReloadAsync()
        {
            var newStatuses = await GetTimeline(new ArrayOptions() { SinceId = sinceId });
            newStatuses.Reverse();
            newStatuses.ForEach(addStatus);
        }

        public async Task<Status> FavouriteAsync(long id) => await client.Favourite(id);
        public async Task<Status> ReblogAsync(long id) => await client.Reblog(id);
        public async Task DeleteAsync(long id) => await client.DeleteStatus(id);

        private void addStatus(Status status)
        {
            statuses.Add(status);
            sinceId = sinceId > status.Id ? sinceId : status.Id;
        }
    }

    class HomeTimelineModel : TimelineModelBase
    {
        protected override Task<MastodonList<Status>> GetTimeline(ArrayOptions options) => client.GetHomeTimeline(options);
        protected override TimelineStreaming GetStreaming() => client.GetUserStreaming();
    }

    class LocalTimelineModel : TimelineModelBase
    {
        protected override Task<MastodonList<Status>> GetTimeline(ArrayOptions options) => client.GetPublicTimeline(options, true);
        protected override TimelineStreaming GetStreaming() => null;
    }

    class FederatedTimelineModel : TimelineModelBase
    {
        protected override Task<MastodonList<Status>> GetTimeline(ArrayOptions options) => client.GetPublicTimeline(options);
        protected override TimelineStreaming GetStreaming() => client.GetPublicStreaming();
    }
}
