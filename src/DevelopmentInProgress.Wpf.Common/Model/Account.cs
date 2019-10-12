using DevelopmentInProgress.MarketView.Interface.Enums;
using DevelopmentInProgress.MarketView.Interface.Interfaces;
using System.Collections.ObjectModel;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.Common.Model
{
    public class Account : EntityBase
    {
        private const string secretText = "**********";
        private string apiSecret;
        private string btcDisplayValue;
        private string usdtDisplayValue;
        private decimal btcValue;
        private decimal usdtValue;

        public Account(Interface.AccountInfo accountInfo)
        {
            AccountInfo = accountInfo;

            if(!string.IsNullOrWhiteSpace(AccountInfo?.User?.ApiSecret))
            {
                apiSecret = secretText;
            }

            Balances = new ObservableCollection<AccountBalance>();

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

        public string ApiPassPhrase
        {
            get { return AccountInfo?.User?.ApiPassPhrase; }
            set
            {
                if (AccountInfo.User.ApiPassPhrase != value)
                {
                    AccountInfo.User.ApiPassPhrase = value;
                    OnPropertyChanged("ApiPassPhrase");
                }
            }
        }

        public Exchange Exchange
        {
            get
            {
                if (AccountInfo?.User != null)
                {
                    return AccountInfo.User.Exchange;
                }
                else
                {
                    return Exchange.Unknown;
                }
            }
            set
            {
                if (AccountInfo.User.Exchange != value)
                {
                    AccountInfo.User.Exchange = value;
                    OnPropertyChanged("Exchange");
                }
            }
        }

        public string BTCDisplayValue
        {
            get { return btcDisplayValue; }
            set
            {
                if (btcDisplayValue != value)
                {
                    btcDisplayValue = value;
                    OnPropertyChanged("BTCDisplayValue");
                }
            }
        }

        public string USDTDisplayValue
        {
            get { return usdtDisplayValue; }
            set
            {
                if (usdtDisplayValue != value)
                {
                    usdtDisplayValue = value;
                    OnPropertyChanged("USDTDisplayValue");
                }
            }
        }

        public decimal BTCValue
        {
            get { return btcValue; }
            set
            {
                if (btcValue != value)
                {
                    btcValue = value;
                    BTCDisplayValue = $"BTC {btcValue}";
                    OnPropertyChanged("BTCValue");
                }
            }
        }

        public decimal USDTValue
        {
            get { return usdtValue; }
            set
            {
                if (usdtValue != value)
                {
                    usdtValue = value;
                    USDTDisplayValue = $"${usdtValue}";
                    OnPropertyChanged("USDTValue");
                }
            }
        }

        public ObservableCollection<AccountBalance> Balances { get; private set; }
    }
}