using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.MarketView.Events;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using System;
using System.Threading;
using System.Windows.Input;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class AccountViewModel : BaseViewModel
    {
        private CancellationTokenSource accountCancellationTokenSource;
        private Account account;
        private AccountBalance selectedAsset;
        private bool isLoggingIn;
        private bool disposed;

        public AccountViewModel(IExchangeService exchangeService)
            : base(exchangeService)
        {
            accountCancellationTokenSource = new CancellationTokenSource();

            LoginCommand = new ViewModelCommand(Login);
        }

        public event EventHandler<AccountEventArgs> OnAccountNotification;

        public ICommand LoginCommand { get; set; }

        public Account Account
        {
            get { return account; }
            set
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
                    OnSelectedAsset(selectedAsset);
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

        public override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (accountCancellationTokenSource != null
                    && !accountCancellationTokenSource.IsCancellationRequested)
                {
                    accountCancellationTokenSource.Cancel();
                }
            }

            disposed = true;
        }

        private async void Login(object param)
        {
            if (string.IsNullOrWhiteSpace(Account.AccountInfo.User.ApiKey)
                || string.IsNullOrWhiteSpace(Account.AccountInfo.User.ApiSecret))
            {
                OnException(new Exception("Both api key and secret key are required to login to an account."));
                return;
            }

            IsLoggingIn = true;

            try
            {
                Account = await ExchangeService.GetAccountInfoAsync(Account.AccountInfo.User.ApiKey, Account.AccountInfo.User.ApiSecret, accountCancellationTokenSource.Token);
            }
            catch(Exception ex)
            {
                OnException(ex);
            }

            IsLoggingIn = false;
        }

        private void OnException(Exception exception)
        {
            var onAccountNotification = OnAccountNotification;
            onAccountNotification?.Invoke(this, new AccountEventArgs { Exception = exception });
        }

        private void OnSelectedAsset(AccountBalance selectedAsset)
        {
            var onAccountNotification = OnAccountNotification;
            onAccountNotification?.Invoke(this, new AccountEventArgs { Value = Account, SelectedAsset = selectedAsset });
        }
    }
}
