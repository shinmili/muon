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
        public ReactiveProperty<StatusViewModel> InReplyTo { get; } = new ReactiveProperty<StatusViewModel>();
        public ReactiveProperty<string> InReplyToText { get; } = new ReactiveProperty<string>("");

        public AsyncReactiveCommand TootCommand { get; }
        public ReactiveCommand CancelReplyCommand { get; }

        private MastodonClient client;

        public MainViewModel()
        {
            client = new MastodonClient(Properties.Settings.Default.AppRegistration, Properties.Settings.Default.Auth);

            TootCommand = Text.Select(t => t.Length > 0)
                .ToAsyncReactiveCommand()
                .WithSubscribe(executeTootCommand);
            CancelReplyCommand = InReplyTo
                .Select(svm => svm != null)
                .ToReactiveCommand()
                .WithSubscribe(() => InReplyTo.Value = null);

            InReplyTo.Subscribe(svm =>
            {
                if (svm == null)
                {
                    InReplyToText.Value = "";
                }
                else
                {
                    InReplyToText.Value = $"To: {svm.OriginalStatus.Account.UserName}: {svm.OriginalStatus.Content}";
                    Text.Value = $"@{svm.OriginalStatus.Account.AccountName} {Text.Value}";
                }
            });
        }

        private async Task executeTootCommand()
        {
            try
            {
                Status status = await client.PostStatus(Text.Value, Mastonet.Visibility.Public, InReplyTo.Value?.Status.Id);
                Text.Value = "";
                InReplyTo.Value = null;
            }
            catch (ServerErrorException e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
