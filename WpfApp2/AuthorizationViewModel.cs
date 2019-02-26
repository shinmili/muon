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
            WaitingForAuthCode = new ReactiveProperty<bool>(false);

            RequestTokenCommand = Instance.Select(x => !string.IsNullOrEmpty(x)).ToReactiveCommand();
            RequestTokenCommand.Subscribe(executeRequestTokenCommand);

            AuthorizeCommand = AccessToken
                .CombineLatest(WaitingForAuthCode, (token, waiting) => waiting && token.Length == 64)
                .ToReactiveCommand();
            AuthorizeCommand.Subscribe(executeAuthorizeCommand);

            OkCommand = new ReactiveCommand();
            OkCommand.Subscribe(executeOkCommand);

            CancelCommand = new ReactiveCommand();
            CancelCommand.Subscribe(executeCancelCommand);
        }

        public ReactiveProperty<string> Instance { get; }
        public ReactiveProperty<string> AccessToken { get; }
        private ReactiveProperty<bool> WaitingForAuthCode;

        #region Commands

        public ReactiveCommand RequestTokenCommand { get; }
        public ReactiveCommand AuthorizeCommand { get; }
        public ReactiveCommand OkCommand { get; }
        public ReactiveCommand CancelCommand { get; }

        private async void executeRequestTokenCommand(object parameter)
        {
            authenticationClient = new AuthenticationClient(Instance.Value);
            try
            {
                settings.AppRegistration = await authenticationClient.CreateApp("Brillenetui", Scope.Read | Scope.Write | Scope.Follow);
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show("Cannot connect to the instance.");
                return;
            }
            Process.Start(authenticationClient.OAuthUrl());
            WaitingForAuthCode.Value = true;
        }

        private async void executeAuthorizeCommand(object parameter)
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

        private void executeOkCommand(object parameter)
        {
            settings.Save();
            MessageBox.Show("Successfully saved.");
            Closing(this, null);
        }

        private void executeCancelCommand(object parameter)
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
