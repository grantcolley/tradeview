using System.Collections.Generic;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.MarketView.Model
{
    public class Account : EntityBase
    {
        private const string secretText = "**********";
        private string apiSecret;

        public Account(Interface.AccountInfo accountInfo)
        {
            AccountInfo = accountInfo;

            if(!string.IsNullOrWhiteSpace(AccountInfo?.User?.ApiSecret))
            {
                apiSecret = secretText;
            }

            Balances = new List<AccountBalance>();

            if (accountInfo.Balances != null)
            {
                foreach (var balance in AccountInfo.Balances)
                {
                    Balances.Add(new AccountBalance { Asset = balance.Asset, Free = balance.Free, Locked = balance.Locked });
                }
            }

            OnPropertyChanged("ApiKey");
            OnPropertyChanged("Balances");
        }

        public Interface.AccountInfo AccountInfo { get; private set; }

        public string ApiKey
        {
            get { return AccountInfo?.User?.ApiKey; }
            set
            {
                if (AccountInfo.User.ApiKey != value)
                {
                    AccountInfo.User.ApiKey = value;
                    OnPropertyChanged("ApiKey");
                }
            }
        }

        public string ApiSecret
        {
            get { return apiSecret; }
            set
            {
                if (AccountInfo.User.ApiSecret != value)
                {
                    AccountInfo.User.ApiSecret = value;
                    apiSecret = string.IsNullOrWhiteSpace(value) ? string.Empty : secretText; 
                    OnPropertyChanged("ApiSecret");
                }
            }
        }

        public List<AccountBalance> Balances { get; private set; }
    }
}