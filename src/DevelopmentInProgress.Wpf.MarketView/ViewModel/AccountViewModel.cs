using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.MarketView.Events;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using Interface = DevelopmentInProgress.MarketView.Interface;

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

        public void SetAccount(Account account)
        {
            try
            {
                if (accountCancellationTokenSource != null
                    && !accountCancellationTokenSource.IsCancellationRequested)
                {
                    accountCancellationTokenSource.Cancel();
                }

                accountCancellationTokenSource = new CancellationTokenSource();

                Account = account;

                OnAccountLoggedOut();
            }
            catch (Exception ex)
            {
                OnException("AccountViewModel.SetAccount", ex);
            }
        }

        public async void Login(object param)
        {
            if (string.IsNullOrWhiteSpace(Account.AccountInfo.User.ApiKey)
                || string.IsNullOrWhiteSpace(Account.AccountInfo.User.ApiSecret))
            {
                OnException("AccountViewModel.Login", new Exception("Both api key and secret key are required to login to an account."));
                return;
            }

            IsLoggingIn = true;

            try
            {
                Account = await ExchangeService.GetAccountInfoAsync(Account.AccountInfo.User.ApiKey, Account.AccountInfo.User.ApiSecret, accountCancellationTokenSource.Token);

                OnAccountLoggedIn(Account);

                ExchangeService.SubscribeAccountInfo(Account.AccountInfo.User, e => AccountInfoUpdate(e.AccountInfo), SubscribeAccountInfoException, accountCancellationTokenSource.Token);
            }
            catch(Exception ex)
            {
                OnException("AccountViewModel.Login", ex);
            }

            IsLoggingIn = false;
        }

        private void SubscribeAccountInfoException(Exception exception)
        {
            OnException("AccountViewModel.Login - ExchangeService.SubscribeAccountInfo", exception);
        }

        private void OnException(string message, Exception exception)
        {
            var onAccountNotification = OnAccountNotification;
            onAccountNotification?.Invoke(this, new AccountEventArgs { Message = message, Exception = exception });
        }

        private void OnAccountLoggedIn(Account account)
        {
            AccountNotification(new AccountEventArgs { Value = Account, AccountEventType = AccountEventType.LoggedIn });
        }
        
        private void OnAccountLoggedOut()
        {
            AccountNotification(new AccountEventArgs { Value = null, AccountEventType = AccountEventType.LoggedOut });
        }

        private void OnSelectedAsset(AccountBalance selectedAsset)
        {
            AccountNotification(new AccountEventArgs { Value = Account, SelectedAsset = selectedAsset, AccountEventType = AccountEventType.SelectedAsset });
        }

        private void AccountNotification(AccountEventArgs args)
        {
            var onAccountNotification = OnAccountNotification;
            onAccountNotification?.Invoke(this, args);
        }

        private void AccountInfoUpdate(Interface.Model.AccountInfo e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.Balances == null
                || !e.Balances.Any())
                {
                    Account.Balances.Clear();
                    return;
                }

                Func<AccountBalance, Interface.Model.AccountBalance, AccountBalance> f = ((ab, nb) =>
                    {
                        ab.Free = nb.Free;
                        ab.Locked = nb.Locked;
                        return ab;
                    });

                var balances = (from ab in Account.Balances
                               join nb in e.Balances on ab.Asset equals nb.Asset
                               select f(ab, nb)).ToList();

                var remove = Account.Balances.Where(ab => !e.Balances.Any(nb => nb.Asset.Equals(ab.Asset))).ToList();
                foreach (var ob in remove)
                {
                    Account.Balances.Remove(ob);
                }

                var add = e.Balances.Where(nb => !Account.Balances.Any(ab => ab.Asset.Equals(nb.Asset))).ToList();
                foreach(var nb in add)
                {
                    Account.Balances.Add(new AccountBalance { Asset = nb.Asset, Free = nb.Free, Locked = nb.Locked });
                }
            });

            AccountNotification(new AccountEventArgs { Value = Account, AccountEventType = AccountEventType.UpdateOrders });
        }
    }
}
