using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Model;

namespace WpfApp2.ViewModel
{
    class AccountTabViewModel : TabContentViewModelBase
    {
        public ReactiveProperty<Account> Account { get; } = new ReactiveProperty<Account>();
        public StatusesViewModel Statuses { get; }

        public AccountTabViewModel(AccountTabParameters param) : base(param)
        {
            GetAccount(param.Id);
            Statuses = new StatusesViewModel(new AccountTimelineModel() { Id = param.Id });
        }

        private async void GetAccount(long id)
        {
            var client = new MastodonClient(Properties.Settings.Default.AppRegistration, Properties.Settings.Default.Auth);
            Account.Value = await client.GetAccount(id);
        }
    }
}
