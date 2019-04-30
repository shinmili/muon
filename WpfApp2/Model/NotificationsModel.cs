using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Model
{
    public class NotificationsModel : PagenatedCollection<Notification>
    {
        private IMastodonClient client;
        private TimelineStreaming streaming;
        private ReactiveProperty<bool> streamingStarted = new ReactiveProperty<bool>();

        /// <summary>
        /// Set this property to start or stop streaming.
        /// </summary>
        public ReactiveProperty<bool> StreamingStarting { get; } = new ReactiveProperty<bool>(false);

        /// <summary>
        /// Tells if the streaming is actually started.
        /// </summary>
        public ReadOnlyReactiveProperty<bool> StreamingStarted { get; }

        public NotificationsModel(IMastodonClient client)
        {
            this.client = client;
            streaming = this.client.GetUserStreaming();
            StreamingStarted = streamingStarted.ToReadOnlyReactiveProperty();
            StreamingStarting.DistinctUntilChanged().Subscribe(OnStreamingChanged);
            streaming.OnNotification += Streaming_OnNotification;
        }

        private void Streaming_OnNotification(object sender, StreamNotificationEventArgs e) => Add(e.Notification);

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
                await streaming.Start();
            }
            catch (TaskCanceledException) { }
            finally { streamingStarted.Value = false; }
        }

        private void StopStreaming() => streaming.Stop();

        protected override Task<MastodonList<Notification>> FetchAsync(ArrayOptions options) => client.GetNotifications(options);
    }
}
