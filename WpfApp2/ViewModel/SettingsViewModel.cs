using Mastonet;
using Newtonsoft.Json;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WpfApp2.Model;

namespace WpfApp2.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
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

        public TabsModel Tabs { get; set; }
        public ReactiveProperty<TabParameters> SelectedTab { get; } = new ReactiveProperty<TabParameters>();

        #region Commands

        public AsyncReactiveCommand RequestTokenCommand { get; }
        public AsyncReactiveCommand AuthorizeCommand { get; }
        public ReactiveCommand OkCommand { get; }
        public ReactiveCommand CancelCommand { get; }

        private async Task executeRequestTokenCommand()
        {
            try
            {
                authenticationClient = new AuthenticationClient(Instance.Value);
                Properties.Settings.Default.AppRegistration = await authenticationClient.CreateApp(Properties.Settings.Default.AppName, Scope.Read | Scope.Write | Scope.Follow);
                Process.Start(authenticationClient.OAuthUrl());
                WaitingForAuthCode.Value = true;
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Cannot connect to the instance.");
            }
        }

        private async Task executeAuthorizeCommand()
        {
            try
            {
                Properties.Settings.Default.Auth = await authenticationClient.ConnectWithCode(AccessToken.Value);
                MessageBox.Show("Successfully authorized!");
                WaitingForAuthCode.Value = false;
            }
            catch (ServerErrorException)
            {
                MessageBox.Show("Authorization failed.");
            }
        }

        private void executeOkCommand()
        {
            Properties.Settings.Default.Save();
            OnClosing(new DialogClosingEventArgs(true));
        }

        private void executeCancelCommand()
        {
            Properties.Settings.Default.Reload();
            OnClosing(new DialogClosingEventArgs(false));
        }
        #endregion

        protected void OnClosing(DialogClosingEventArgs e) => Closing?.Invoke(this, e);
    }

    public class DialogClosingEventArgs : EventArgs
    {
        public bool DialogResult { get; private set; }
        public DialogClosingEventArgs(bool dialogResult) { DialogResult = dialogResult; }
    }
}
