﻿using Mastonet;
using Mastonet.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Muon.Model;

namespace Muon.ViewModel
{
    public class AccountTabViewModel : TabContentViewModelBase
    {
        public ReactiveProperty<Account> Account { get; } = new ReactiveProperty<Account>();
        public ReactiveProperty<string> FullName { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> FollowingUrl { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> FollowersUrl { get; } = new ReactiveProperty<string>();
        public StatusesViewModel Statuses { get; }

        public AccountTabViewModel(AccountTabParameters param, IReactiveProperty<Status> inReplyTo, TabsModel tabs, IMastodonClient client) : base(param, inReplyTo)
        {
            GetAccount(param.Id);
            Statuses = new StatusesViewModel(new AccountTimelineModel(param.Id, client), inReplyTo, tabs);
        }

        private async void GetAccount(long id)
        {
            var client = new MastodonClient(Properties.Settings.Default.AppRegistration, Properties.Settings.Default.Auth);
            Account.Value = await client.GetAccount(id);
            FullName.Value = Account.Value.AccountName;
            if (!FullName.Value.Contains('@'))
            {
                Regex regex = new Regex("^https://(?<domain>[^/]*)/");
                Match match = regex.Match(Account.Value.ProfileUrl);
                FullName.Value += '@' + match.Groups["domain"].Value;
            }

            FollowingUrl.Value = Account.Value.ProfileUrl.Replace("@", "users/") + "/following";
            FollowersUrl.Value = Account.Value.ProfileUrl.Replace("@", "users/") + "/followers";
        }
    }
}
