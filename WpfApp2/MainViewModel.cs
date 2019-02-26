using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp2
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TimelineModel timeline = new TimelineModel();
        private SettingsModel settings = new SettingsModel();
        private MastodonClient client;

        public MainViewModel()
        {
            client = new MastodonClient(settings.AppRegistration, settings.Auth);
            Text = new ReactiveProperty<string>("");
            Posting = new ReactiveProperty<bool>(false);

            TootCommand = Text
                .CombineLatest(Posting, (t, p) => t.Length > 0 && !p)
                .ToReactiveCommand();
            TootCommand.Subscribe(executeTootCommand);
        }

        public ReactiveProperty<string> Text { get; }
        private ReactiveProperty<bool> Posting;
        public ReactiveCommand TootCommand { get; }

        #region Commands

        private async void executeTootCommand(object parameter)
        {
            Posting.Value = true;
            try
            {
                Status status = await client.PostStatus(Text.Value, Mastonet.Visibility.Public);
                Text.Value = "";
            }
            catch (ServerErrorException e)
            {
                MessageBox.Show("ServerErrorException.");
            }
            finally
            {
                Posting.Value = false;
            }
        }

        #endregion
    }
}
