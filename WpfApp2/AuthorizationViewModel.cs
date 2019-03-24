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
    class AuthorizationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler Closing;

        private AuthenticationClient authenticationClient;
        private SettingsModel settings = new SettingsModel();

        public AuthorizationViewModel()
        {
            Instance = new ReactiveProperty<string>(settings.AppRegistration?.Instance);
            AccessToken = new ReactiveProperty<string>(settings.Auth?.AccessToken ?? "");

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
                settings.AppRegistration = await authenticationClient.CreateApp(settings.AppName, Scope.Read | Scope.Write | Scope.Follow);
            }
            catch (HttpRequestException e)
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
                settings.Auth = await authenticationClient.ConnectWithCode(AccessToken.Value);
            }
            catch (ServerErrorException e)
            {
                MessageBox.Show("Authorization failed.");
                return;
            }
            MessageBox.Show(JsonConvert.SerializeObject(settings.Auth));
            WaitingForAuthCode.Value = false;
        }

        private void executeOkCommand()
        {
            settings.Save();
            MessageBox.Show("Successfully saved.");
            Closing(this, null);
        }

        private void executeCancelCommand()
        {
            settings.Reload();
            Closing(this, null);
        }
        #endregion

        protected void OnClosing(EventArgs e)
        {
            var h = Closing;
            h?.Invoke(this, null);
        }
    }
}
