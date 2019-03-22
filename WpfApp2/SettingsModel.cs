using Mastonet;
using Mastonet.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    class SettingsModel
    {
        private AppRegistration appRegistration = Properties.Settings.Default.AppRegistration;
        public AppRegistration AppRegistration
        {
            get => appRegistration;
            set
            {
                appRegistration = value;
                Properties.Settings.Default.AppRegistration = appRegistration;
            }
        }

        private Auth auth = Properties.Settings.Default.Auth;
        public Auth Auth
        {
            get => auth;
            set
            {
                auth = value;
                Properties.Settings.Default.Auth = auth;
            }
        }

        internal void Save() => Properties.Settings.Default.Save();
        internal void Reload() => Properties.Settings.Default.Reload();
    }
}
