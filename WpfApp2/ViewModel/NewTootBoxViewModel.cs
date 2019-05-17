using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Muon.Model;

namespace Muon.ViewModel
{
    public class NewTootBoxViewModel : INotifyPropertyChanged
    {
        private IMastodonClient client;
        private IReactiveProperty<Status> inReplyTo;

        public event PropertyChangedEventHandler PropertyChanged;

        public ReadOnlyReactiveProperty<Status> InReplyTo { get; }
        public ReactiveProperty<string> Text { get; } = new ReactiveProperty<string>("");
        public ReactiveProperty<string> InReplyToText { get; } = new ReactiveProperty<string>("");

        public AsyncReactiveCommand TootCommand { get; }
        public ReactiveCommand CancelReplyCommand { get; }

        public NewTootBoxViewModel(IReactiveProperty<Status> inReplyTo, IMastodonClient client)
        {
            this.inReplyTo = inReplyTo;
            this.client = client;
            InReplyTo = inReplyTo.ToReadOnlyReactiveProperty();

            TootCommand = Text.Select(t => t.Length > 0)
                .ToAsyncReactiveCommand()
                .WithSubscribe(executeTootCommand);
            CancelReplyCommand = this.inReplyTo
                .Select(svm => svm != null)
                .ToReactiveCommand()
                .WithSubscribe(() => this.inReplyTo.Value = null);

            this.inReplyTo.Subscribe(status =>
            {
                if (status == null)
                {
                    InReplyToText.Value = null;
                }
                else
                {
                    Status OriginalStatus = status.Reblog ?? status;
                    InReplyToText.Value = $"To: {OriginalStatus.Account.UserName}: {OriginalStatus.Content}";
                    Text.Value = $"@{OriginalStatus.Account.AccountName} {Text.Value}";
                }
            });
        }

        private async Task executeTootCommand()
        {
            try
            {
                Status status = await client.PostStatus(Text.Value, Mastonet.Visibility.Public, inReplyTo.Value?.Id);
                Text.Value = "";
                inReplyTo.Value = null;
            }
            catch (ServerErrorException e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
