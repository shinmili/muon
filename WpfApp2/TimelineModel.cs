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
    class TimelineModel
    {
        private MastodonClient client;
        private SettingsModel settings;
        private ReactiveCollection<Status> statuses;
        private TimelineStreaming streaming;
        public ReactiveProperty<bool> IsStreaming = new ReactiveProperty<bool>(false);
        private long? sinceId;

        public ReadOnlyReactiveCollection<Status> Statuses { get; }

        public TimelineModel()
        {
            settings = new SettingsModel();
            client = new MastodonClient(settings.AppRegistration, settings.Auth);
            statuses = new ReactiveCollection<Status>();
            streaming = client.GetUserStreaming();
            streaming.OnUpdate += Streaming_OnUpdate;
            Statuses = statuses.ToReadOnlyReactiveCollection();
        }

        public async Task StartStreamingAsync()
        {
            if (IsStreaming.Value) return;
            IsStreaming.Value = true;
            try { await streaming.Start(); }
            catch (TaskCanceledException) { }
        }

        private void Streaming_OnUpdate(object sender, StreamUpdateEventArgs e)
        {
            addStatus(e.Status);
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
            var newStatuses = await client.GetHomeTimeline(null, sinceId);
            newStatuses.Reverse();
            newStatuses.ForEach(addStatus);
        }

        private void addStatus(Status status)
        {
            statuses.Add(status);
            sinceId = status.Id;
        }
    }
}
