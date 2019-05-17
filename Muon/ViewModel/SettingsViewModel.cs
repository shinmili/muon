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
using Muon.Model;

namespace Muon.ViewModel
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

            ShowHomeTimelineTab = new ReactiveProperty<bool>(Properties.Settings.Default.Tabs?.OfType<TimelineTabParameters>()?.Any(x => x.Type == TimelineType.Home) ?? false);
            ShowLocalTimelineTab = new ReactiveProperty<bool>(Properties.Settings.Default.Tabs?.OfType<TimelineTabParameters>()?.Any(x => x.Type == TimelineType.Local) ?? false);
            ShowFederatedTimelineTab = new ReactiveProperty<bool>(Properties.Settings.Default.Tabs?.OfType<TimelineTabParameters>()?.Any(x => x.Type == TimelineType.Federated) ?? false);
            ShowNotificationsTab = new ReactiveProperty<bool>(Properties.Settings.Default.Tabs?.OfType<NotificationTabParameters>()?.Any() ?? false);

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
        public string AuthorizeButtonText => string.Format(Properties.Resources.AuthorizeAppButtonText, Properties.Settings.Default.AppName);
        private ReactiveProperty<bool> WaitingForAuthCode = new ReactiveProperty<bool>(false);

        public ReactiveProperty<bool> ShowHomeTimelineTab { get; }
        public ReactiveProperty<bool> ShowNotificationsTab { get; }
        public ReactiveProperty<bool> ShowLocalTimelineTab { get; }
        public ReactiveProperty<bool> ShowFederatedTimelineTab { get; }

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
                MessageBox.Show(Properties.Resources.InitialServerConnectionFailedMessage);
            }
        }

        private async Task executeAuthorizeCommand()
        {
            try
            {
                Properties.Settings.Default.Auth = await authenticationClient.ConnectWithCode(AccessToken.Value);
                MessageBox.Show(Properties.Resources.AuthorizedMessage);
                WaitingForAuthCode.Value = false;
            }
            catch (ServerErrorException)
            {
                MessageBox.Show(Properties.Resources.AuthorizationFailedMessage);
            }
        }

        private void executeOkCommand()
        {
            Properties.Settings.Default.Save();
            if (ShowHomeTimelineTab.Value)
            {
                Tabs.OpenIfNotPresent(new TimelineTabParameters() { Name = "Home", Type = TimelineType.Home });
            }
            else
            {
                Tabs.CloseTab(new TimelineTabParameters() { Type = TimelineType.Home });
            }
            if (ShowNotificationsTab.Value)
            {
                Tabs.OpenIfNotPresent(new NotificationTabParameters() { Name = "Notifications" });
            }
            else
            {
                Tabs.CloseTab(new NotificationTabParameters());
            }
            if (ShowLocalTimelineTab.Value)
            {
                Tabs.OpenIfNotPresent(new TimelineTabParameters() { Name = "Local", Type = TimelineType.Local });
            }
            else
            {
                Tabs.CloseTab(new TimelineTabParameters() { Type = TimelineType.Local });
            }
            if (ShowFederatedTimelineTab.Value)
            {
                Tabs.OpenIfNotPresent(new TimelineTabParameters() { Name = "Federated", Type = TimelineType.Federated });
            }
            else
            {
                Tabs.CloseTab(new TimelineTabParameters() { Type = TimelineType.Federated });
            }
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
