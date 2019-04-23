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

namespace WpfApp2.Model
{
    abstract class TimelineModelBase : PagenatedCollection<Status>
    {
        protected MastodonClient client = new MastodonClient(Properties.Settings.Default.AppRegistration, Properties.Settings.Default.Auth);
        private TimelineStreaming streaming;

        /// <summary>
        /// Set this property to start or stop streaming.
        /// </summary>
        public ReactiveProperty<bool> StreamingStarting { get; } = new ReactiveProperty<bool>(false);

        /// <summary>
        /// Tells if the streaming is actually started.
        /// </summary>
        public ReadOnlyReactiveProperty<bool> StreamingStarted { get; }

        public abstract bool IsStreamingAvailable { get; }

        private ReactiveProperty<bool> streamingStarted = new ReactiveProperty<bool>(false);

        protected abstract Task<MastodonList<Status>> GetTimeline(ArrayOptions options);
        protected override Task<MastodonList<Status>> FetchAsync(ArrayOptions options) => GetTimeline(options);
        protected abstract TimelineStreaming GetStreaming();

        public event EventHandler<StreamNotificationEventArgs> OnNotification
        {
            add { streaming.OnNotification += value; }
            remove { streaming.OnNotification -= value; }
        }

        protected TimelineModelBase()
        {
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
            int? index = this.Select((s, i) => new { s, i })
                .FirstOrDefault(x => x.s.Id == e.StatusId)
                ?.i;
            if (index.HasValue) RemoveAt(index.Value);
        }

        private void Streaming_OnUpdate(object sender, StreamUpdateEventArgs e) => Add(e.Status);

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

        public async Task<Status> FavouriteAsync(long id) => await client.Favourite(id);
        public async Task<Status> ReblogAsync(long id) => await client.Reblog(id);
        public async Task DeleteAsync(long id) => await client.DeleteStatus(id);
    }

    class HomeTimelineModel : TimelineModelBase
    {
        public override bool IsStreamingAvailable => true;
        protected override Task<MastodonList<Status>> GetTimeline(ArrayOptions options) => client.GetHomeTimeline(options);
        protected override TimelineStreaming GetStreaming() => client.GetUserStreaming();
    }

    class LocalTimelineModel : TimelineModelBase
    {
        public override bool IsStreamingAvailable => false;
        protected override Task<MastodonList<Status>> GetTimeline(ArrayOptions options) => client.GetPublicTimeline(options, true);
        protected override TimelineStreaming GetStreaming() => null;
    }

    class FederatedTimelineModel : TimelineModelBase
    {
        public override bool IsStreamingAvailable => true;
        protected override Task<MastodonList<Status>> GetTimeline(ArrayOptions options) => client.GetPublicTimeline(options);
        protected override TimelineStreaming GetStreaming() => client.GetPublicStreaming();
    }

    class AccountTimelineModel : TimelineModelBase
    {
        public override bool IsStreamingAvailable => false;
        public long Id { get; set; }
        protected override Task<MastodonList<Status>> GetTimeline(ArrayOptions options) => client.GetAccountStatuses(Id, options);
        protected override TimelineStreaming GetStreaming() => null;
    }

    public enum TimelineType { Home, Local, Federated }
}
