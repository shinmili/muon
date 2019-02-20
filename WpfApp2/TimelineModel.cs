using Mastonet;
using Mastonet.Entities;
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
        private ObservableCollection<Status> statuses;
        private TimelineStreaming streaming;
        private long? sinceId;

        public ReadOnlyObservableCollection<Status> Statuses { get; }

        public TimelineModel()
        {
            settings = new SettingsModel();
            client = new MastodonClient(settings.AppRegistration, settings.Auth);
            statuses = new ObservableCollection<Status>();
            streaming = client.GetUserStreaming();
            Statuses = new ReadOnlyObservableCollection<Status>(statuses);
        }

        public async Task StartStreamingAsync()
        {
            streaming.OnUpdate += (s, e) => addStatus(e.Status);
            await streaming.Start();
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
