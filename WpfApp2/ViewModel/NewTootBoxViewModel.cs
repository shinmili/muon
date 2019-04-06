using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Model;

namespace WpfApp2.ViewModel
{
    class NewTootBoxViewModel
    {
        private MastodonClient client;
        private InReplyToModel inReplyToModel = InReplyToModel.Instance;

        public ReactiveProperty<string> Text { get; } = new ReactiveProperty<string>("");
        public ReactiveProperty<string> InReplyToText { get; } = new ReactiveProperty<string>("");

        public AsyncReactiveCommand TootCommand { get; }
        public ReactiveCommand CancelReplyCommand { get; }

        public NewTootBoxViewModel()
        {
            client = new MastodonClient(Properties.Settings.Default.AppRegistration, Properties.Settings.Default.Auth);

            TootCommand = Text.Select(t => t.Length > 0)
                .ToAsyncReactiveCommand()
                .WithSubscribe(executeTootCommand);
            CancelReplyCommand = inReplyToModel.InReplyTo
                .Select(svm => svm != null)
                .ToReactiveCommand()
                .WithSubscribe(() => inReplyToModel.InReplyTo.Value = null);

            inReplyToModel.InReplyTo.Subscribe(status =>
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
                Status status = await client.PostStatus(Text.Value, Mastonet.Visibility.Public, inReplyToModel.InReplyTo.Value?.Id);
                Text.Value = "";
                inReplyToModel.InReplyTo.Value = null;
            }
            catch (ServerErrorException e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
