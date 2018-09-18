using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.Common.Services;
using DevelopmentInProgress.Wpf.Common.ViewModel;
using DevelopmentInProgress.Wpf.StrategyManager.Events;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Wpf.StrategyManager.ViewModel
{
    public class AccountViewModel : BaseViewModel
    {
        IWpfExchangeService exchangeService;
        private CancellationTokenSource accountCancellationTokenSource;
        private Account account;
        private bool isLoggingIn;
        private bool disposed;
        
        public AccountViewModel(IWpfExchangeService exchangeService)
        {
            this.exchangeService = exchangeService;

            accountCancellationTokenSource = new CancellationTokenSource();
        }

        public event EventHandler<AccountEventArgs> OnAccountNotification;

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

        public async Task Login(Account accountLogin)
        {
            if (string.IsNullOrWhiteSpace(accountLogin.AccountInfo.User.ApiKey)
                || string.IsNullOrWhiteSpace(accountLogin.AccountInfo.User.ApiSecret))
            {
                var exmsg = "Both api key and secret key are required to login to an account.";
                OnException($"AccountViewModel.Login {exmsg}", new Exception(exmsg));
                return;
            }

            IsLoggingIn = true;

            try
            {
                Account = await exchangeService.GetAccountInfoAsync(accountLogin.AccountInfo.User.ApiKey, accountLogin.AccountInfo.User.ApiSecret, accountCancellationTokenSource.Token);

                exchangeService.SubscribeAccountInfo(Account.AccountInfo.User, e => AccountInfoUpdate(e.AccountInfo), SubscribeAccountInfoException, accountCancellationTokenSource.Token);
            }
            catch (Exception ex)
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

        private void AccountNotification()
        {
            var onAccountNotification = OnAccountNotification;
            onAccountNotification?.Invoke(this, new AccountEventArgs());
        }

        private void AccountInfoUpdate(MarketView.Interface.Model.AccountInfo e)
        {
            Action<MarketView.Interface.Model.AccountInfo> action = aie =>
            {
                if (aie.Balances == null
                    || !aie.Balances.Any())
                {
                    Account.Balances.Clear();
                    return;
                }

                Func<AccountBalance, MarketView.Interface.Model.AccountBalance, AccountBalance> f = ((ab, nb) =>
                {
                    ab.Free = nb.Free;
                    ab.Locked = nb.Locked;
                    return ab;
                });

                var balances = (from ab in Account.Balances
                                join nb in aie.Balances on ab.Asset equals nb.Asset
                                select f(ab, nb)).ToList();

                var remove = Account.Balances.Where(ab => !aie.Balances.Any(nb => nb.Asset.Equals(ab.Asset))).ToList();
                foreach (var ob in remove)
                {
                    Account.Balances.Remove(ob);
                }

                var add = aie.Balances.Where(nb => !Account.Balances.Any(ab => ab.Asset.Equals(nb.Asset))).ToList();
                foreach (var nb in add)
                {
                    Account.Balances.Add(new AccountBalance { Asset = nb.Asset, Free = nb.Free, Locked = nb.Locked });
                }
            };

            if (Dispatcher == null)
            {
                action(e);
            }
            else
            {
                Dispatcher.Invoke(action, e);
            }

            AccountNotification();
        }
    }
}
