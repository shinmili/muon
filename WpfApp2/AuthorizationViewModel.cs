using Mastonet;
using Mastonet.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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

        #region INotifyPropertyChanged implementations

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string instance;
        public string Instance
        {
            get => instance;
            set
            {
                instance = value;
                NotifyPropertyChanged();
                RequestTokenCommand.RaiseCanExecuteChanged();
            }
        }

        private string accessToken;
        public string AccessToken
        {
            get => accessToken;
            set
            {
                accessToken = value;
                NotifyPropertyChanged();
                AuthorizeCommand.RaiseCanExecuteChanged();
            }
        }

        private bool waitingForAuthCode = false;
        public bool WaitingForAuthCode
        {
            get => waitingForAuthCode;
            private set
            {
                waitingForAuthCode = value;
                NotifyPropertyChanged();
                AuthorizeCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Commands

        private DelegateCommand requestTokenCommand;
        public DelegateCommand RequestTokenCommand
        {
            get => requestTokenCommand ?? (requestTokenCommand = new DelegateCommand
            {
                ExecuteHandler = executeRequestTokenCommand,
                CanExecuteHandler = canExecuteRequestTokenCommand,
            });
        }

        private async void executeRequestTokenCommand(object parameter)
        {
            authenticationClient = new AuthenticationClient(Instance);
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
            WaitingForAuthCode = true;
        }

        private bool canExecuteRequestTokenCommand(object parameter) => !string.IsNullOrEmpty(Instance);

        private DelegateCommand authorizeCommand;
        public DelegateCommand AuthorizeCommand
        {
            get => authorizeCommand ?? (authorizeCommand = new DelegateCommand
            {
                ExecuteHandler = executeAuthorizeCommand,
                CanExecuteHandler = canExecuteAuthorizeCommand,
            });
        }

        private async void executeAuthorizeCommand(object parameter)
        {
            try
            {
                settings.Auth = await authenticationClient.ConnectWithCode(AccessToken);
            }
            catch (ServerErrorException e)
            {
                MessageBox.Show("Authorization failed.");
                return;
            }
            MessageBox.Show(JsonConvert.SerializeObject(settings.Auth));
            WaitingForAuthCode = false;
        }

        private bool canExecuteAuthorizeCommand(object parameter)
        {
            return WaitingForAuthCode && AccessToken.Length == 64;
        }

        private DelegateCommand okCommand;
        public DelegateCommand OkCommand
        {
            get => okCommand ?? (okCommand = new DelegateCommand
            {
                ExecuteHandler = executeOkCommand,
                CanExecuteHandler = null,
            });
        }

        private void executeOkCommand(object parameter)
        {
            settings.Save();
            MessageBox.Show("Successfully saved.");
            Closing(this, null);
        }

        private DelegateCommand cancelCommand;
        public DelegateCommand CancelCommand
        {
            get => cancelCommand ?? (cancelCommand = new DelegateCommand
            {
                ExecuteHandler = executeCancelCommand,
                CanExecuteHandler = null,
            });
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

        public void LoadSettingsToUi()
        {
            Instance = settings.AppRegistration.Instance;
            AccessToken = settings.Auth.AccessToken;
        }
    }
}
