using Mastonet;
using Newtonsoft.Json;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace WpfApp2
{
    class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DialogClosingEventArgs> Closing;

        private AuthenticationClient authenticationClient;

        public SettingsViewModel()
        {
            Instance = new ReactiveProperty<string>(Properties.Settings.Default.AppRegistration?.Instance);
            AccessToken = new ReactiveProperty<string>(Properties.Settings.Default.Auth?.AccessToken ?? "");

            RequestTokenCommand = Instance
                .Select(x => !string.IsNullOrEmpty(x))
                .ToAsyncReactiveCommand()
                .WithSubscribe(executeRequestTokenCommand);

            AuthorizeCommand = AccessToken
                .CombineLatest(WaitingForAuthCode, (token, waiting) => waiting && token.Length == 64)
                .ToAsyncReactiveCommand()
                .WithSubscribe(executeAuthorizeCommand);

            OkCommand = new ReactiveCommand().WithSubscribe(executeOkCommand);
            CancelCommand = new ReactiveCommand().WithSubscribe(executeCancelCommand);
        }

        public ReactiveProperty<string> Instance { get; }
        public ReactiveProperty<string> AccessToken { get; }
        private ReactiveProperty<bool> WaitingForAuthCode = new ReactiveProperty<bool>(false);

        #region Commands

        public AsyncReactiveCommand RequestTokenCommand { get; }
        public AsyncReactiveCommand AuthorizeCommand { get; }
        public ReactiveCommand OkCommand { get; }
        public ReactiveCommand CancelCommand { get; }

        private async Task executeRequestTokenCommand()
        {
            authenticationClient = new AuthenticationClient(Instance.Value);
            try
            {
                Properties.Settings.Default.AppRegistration = await authenticationClient.CreateApp(Properties.Settings.Default.AppName, Scope.Read | Scope.Write | Scope.Follow);
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Cannot connect to the instance.");
                return;
            }
            Process.Start(authenticationClient.OAuthUrl());
            WaitingForAuthCode.Value = true;
        }

        private async Task executeAuthorizeCommand()
        {
            try
            {
                Properties.Settings.Default.Auth = await authenticationClient.ConnectWithCode(AccessToken.Value);
            }
            catch (ServerErrorException)
            {
                MessageBox.Show("Authorization failed.");
                return;
            }
            MessageBox.Show(JsonConvert.SerializeObject(Properties.Settings.Default.Auth));
            WaitingForAuthCode.Value = false;
        }

        private void executeOkCommand()
        {
            Properties.Settings.Default.Save();
            MessageBox.Show("Successfully saved.");
            Closing(this, new DialogClosingEventArgs(true));
        }

        private void executeCancelCommand()
        {
            Properties.Settings.Default.Reload();
            Closing(this, new DialogClosingEventArgs(false));
        }
        #endregion

        protected void OnClosing(DialogClosingEventArgs e) => Closing?.Invoke(this, e);
    }

    class DialogClosingEventArgs : EventArgs
    {
        public bool DialogResult { get; private set; }
        public DialogClosingEventArgs(bool dialogResult) { DialogResult = dialogResult; }
    }
}
