using Mastonet;
using Mastonet.Entities;
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

        public NavigationWindow Navigation { get; set; }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //MessageBox.Show((string)typeof(AuthorizationViewModel).GetProperty(propertyName).GetValue(this));
        }

        private string instance = "";
        public string Instance
        {
            get => instance;
            set
            {
                instance = value;
                NotifyPropertyChanged();
                OpenAuthorizationPage.RaiseCanExecuteChanged();
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
            }
        }

        private DelegateCommand openAuthorizationPage;
        public DelegateCommand OpenAuthorizationPage
        {
            get
            {
                if (openAuthorizationPage == null)
                {
                    openAuthorizationPage = new DelegateCommand
                    {
                        ExecuteHandler = executeOpenAuthorizationPage,
                        CanExecuteHandler = canExecuteOpenAuthorizationPage,
                    };
                }
                return openAuthorizationPage;
            }
        }

        private async void executeOpenAuthorizationPage(object parameter)
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
            Navigation.Navigate(new Uri("EnterAuthCodePage.xaml", UriKind.Relative));
        }

        private bool canExecuteOpenAuthorizationPage(object parameter) => !string.IsNullOrEmpty(Instance);

        private DelegateCommand authorizeWithCode;
        public DelegateCommand AuthorizeWithCode
        {
            get
            {
                if (authorizeWithCode == null)
                {
                    authorizeWithCode = new DelegateCommand
                    {
                        ExecuteHandler = executeAuthorizeWithCode,
                        CanExecuteHandler = canExecuteAuthorizeWithCode,
                    };
                }
                return authorizeWithCode;
            }
        }

        private async void executeAuthorizeWithCode(object parameter)
        {
            await authenticationClient.ConnectWithCode(AccessToken);
        }

        private bool canExecuteAuthorizeWithCode(object parameter)
        {
            return true;
        }
    }
}
