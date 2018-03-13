using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using System;
using System.Windows.Input;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class AccountViewModel : BaseViewModel
    {
        private Account account;
        private AccountBalance selectedAsset;
        private Action<Exception> exception;
        private bool isLoggingIn;
        private bool disposed;

        public AccountViewModel(Account account, IExchangeService exchangeService, Action<Exception> exception)
            : base(exchangeService)
        {
            Account = account;

            this.exception = exception;

            LoginCommand = new ViewModelCommand(Login);
        }

        public ICommand LoginCommand { get; set; }

        public Account Account
        {
            get { return account; }
            private set
            {
                if (account != value)
                {
                    account = value;
                    OnPropertyChanged("Account");
                }
            }
        }

        public AccountBalance SelectedAsset
        {
            get { return selectedAsset; }
            set
            {
                if (selectedAsset != value)
                {
                    selectedAsset = value;
                    OnPropertyChanged("SelectedAsset");
                }
            }
        }

        public bool IsLoggingIn
        {
            get { return isLoggingIn; }
            set
            {
                if (isLoggingIn != value)
                {
                    isLoggingIn = value;
                    OnPropertyChanged("IsLoggingIn");
                }
            }
        }

        private async void Login(object param)
        {
            if (string.IsNullOrWhiteSpace(Account.AccountInfo.User.ApiKey)
                || string.IsNullOrWhiteSpace(Account.AccountInfo.User.ApiSecret))
            {
                exception.Invoke(new Exception("Both api key and secret key are required to login to an account."));
                return;
            }

            IsLoggingIn = true;

            try
            {
                Account = await ExchangeService.GetAccountInfoAsync(Account.AccountInfo.User.ApiKey, Account.AccountInfo.User.ApiSecret);
            }
            catch(Exception e)
            {
                exception.Invoke(e);
            }

            IsLoggingIn = false;
        }

        public override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // do tuff here...
            }

            disposed = true;
        }
    }
}
