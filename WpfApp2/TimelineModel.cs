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
        public ReadOnlyObservableCollection<Status> Statuses { get; }

        public TimelineModel()
        {
            settings = new SettingsModel();
            client = new MastodonClient(settings.AppRegistration, settings.Auth);
            statuses = new ObservableCollection<Status>();
            Statuses = new ReadOnlyObservableCollection<Status>(statuses);
        }

        public async Task ReloadAsync()
        {
            long? sinceId = statuses.LastOrDefault()?.Id;
            var newStatuses = await client.GetHomeTimeline(null, sinceId);
            newStatuses.Reverse();
            foreach (var newStatus in newStatuses)
            {
                statuses.Add(newStatus);
            }
        }
    }
}
