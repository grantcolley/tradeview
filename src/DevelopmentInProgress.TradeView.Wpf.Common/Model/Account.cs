using DevelopmentInProgress.TradeView.Core.Enums;
using System.Collections.ObjectModel;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class Account : EntityBase
    {
        private const string secretText = "**********";
        private string apiSecret;
        private string btcDisplayValue;
        private string equalsDisplayValue;
        private string usdtDisplayValue;
        private decimal btcValue;
        private decimal usdtValue;

        public string AccountName
        {
            get { return AccountInfo?.User?.AccountName; }
            set
            {
                if (AccountInfo.User.AccountName != value)
                {
                    AccountInfo.User.AccountName = value;
                    OnPropertyChanged(nameof(AccountName));
                }
            }
        }

        public Account(Core.Model.AccountInfo accountInfo)
        {
            AccountInfo = accountInfo;

            if(!string.IsNullOrWhiteSpace(AccountInfo?.User?.ApiSecret))
            {
                apiSecret = secretText;
            }

            Balances = new ObservableCollection<AccountBalance>();

            foreach (var balance in AccountInfo.Balances)
            {
                Balances.Add(new AccountBalance { Asset = balance.Asset, Free = balance.Free, Locked = balance.Locked });
            }

            OnPropertyChanged(nameof(ApiKey));
            OnPropertyChanged(nameof(Balances));
        }

        public Core.Model.AccountInfo AccountInfo { get; private set; }

        public string ApiKey
        {
            get { return AccountInfo?.User?.ApiKey; }
            set
            {
                if (AccountInfo.User.ApiKey != value)
                {
                    AccountInfo.User.ApiKey = value;
                    OnPropertyChanged(nameof(ApiKey));
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
                    OnPropertyChanged(nameof(ApiSecret));
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
                    OnPropertyChanged(nameof(ApiPassPhrase));
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
                    OnPropertyChanged(nameof(Exchange));
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
                    OnPropertyChanged(nameof(BTCDisplayValue));
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

                    if(string.IsNullOrWhiteSpace(EqualsDisplayValue))
                    {
                        EqualsDisplayValue = "=";
                    }

                    OnPropertyChanged(nameof(USDTDisplayValue));
                }
            }
        }

        public string EqualsDisplayValue
        {
            get { return equalsDisplayValue; }
            set
            {
                if (equalsDisplayValue != value)
                {
                    equalsDisplayValue = value;
                    OnPropertyChanged(nameof(EqualsDisplayValue));
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
                    OnPropertyChanged(nameof(BTCValue));
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
                    OnPropertyChanged(nameof(USDTValue));
                }
            }
        }

        public ObservableCollection<AccountBalance> Balances { get; private set; }
    }
}