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

        public ReactiveProperty<string> Text { get; } = new ReactiveProperty<string>("");
        public AsyncReactiveCommand TootCommand { get; }

        private SettingsModel settings = new SettingsModel();
        private MastodonClient client;

        public MainViewModel()
        {
            client = new MastodonClient(settings.AppRegistration, settings.Auth);

            TootCommand = Text.Select(t => t.Length > 0)
                .ToAsyncReactiveCommand()
                .WithSubscribe(executeTootCommand);
        }

        private async Task executeTootCommand()
        {
            try
            {
                Status status = await client.PostStatus(Text.Value, Mastonet.Visibility.Public);
                Text.Value = "";
            }
            catch (ServerErrorException e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
