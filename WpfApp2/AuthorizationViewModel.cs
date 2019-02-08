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

        private AuthenticationClient authenticationClient;
        private AppRegistration appRegistration;
        private Auth auth;

        #region INotifyPropertyChanged implementations

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string instance = "";
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

        private string accessToken = "";
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
            get
            {
                if (requestTokenCommand == null)
                {
                    requestTokenCommand = new DelegateCommand
                    {
                        ExecuteHandler = executeRequestTokenCommand,
                        CanExecuteHandler = canExecuteRequestTokenCommand,
                    };
                }
                return requestTokenCommand;
            }
        }

        private async void executeRequestTokenCommand(object parameter)
        {
            authenticationClient = new AuthenticationClient(Instance);
            try
            {
                appRegistration = await authenticationClient.CreateApp("Brillenetui", Scope.Read | Scope.Write | Scope.Follow);
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
            get
            {
                if (authorizeCommand == null)
                {
                    authorizeCommand = new DelegateCommand
                    {
                        ExecuteHandler = executeAuthorizeCommand,
                        CanExecuteHandler = canExecuteAuthorizeCommand,
                    };
                }
                return authorizeCommand;
            }
        }

        private async void executeAuthorizeCommand(object parameter)
        {
            auth = await authenticationClient.ConnectWithCode(AccessToken);
        }

        private bool canExecuteAuthorizeCommand(object parameter)
        {
            return WaitingForAuthCode && AccessToken.Length == 64;
        }

        #endregion
    }
}
