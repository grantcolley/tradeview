using DevelopmentInProgress.TradeView.Wpf.Common.Cache;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Common.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Strategies.Events;
using Prism.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DevelopmentInProgress.TradeView.Wpf.Strategies.ViewModel
{
    public class AccountViewModel : ExchangeViewModel
    {
        private CancellationTokenSource accountCancellationTokenSource;
        private DispatcherTimer dispatcherTimer;
        private ISymbolsCacheFactory symbolsCacheFactory;
        private ISymbolsCache symbolsCache;
        private Account account;
        private bool isLoggingIn;
        private bool disposed;
        private object balancesLock = new object();
            
        public AccountViewModel(IWpfExchangeService exchangeService, ISymbolsCacheFactory symbolsCacheFactory, ILoggerFacade logger)
            : base(exchangeService, logger)
        {
            accountCancellationTokenSource = new CancellationTokenSource();

            this.symbolsCacheFactory = symbolsCacheFactory;
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
                Account = await ExchangeService.GetAccountInfoAsync(accountLogin.Exchange, accountLogin.AccountInfo.User, accountCancellationTokenSource.Token);

                ExchangeService.SubscribeAccountInfo(accountLogin.Exchange, Account.AccountInfo.User, e => AccountInfoUpdate(e.AccountInfo), SubscribeAccountInfoException, accountCancellationTokenSource.Token);

                AccountNotification(AccountEventType.SetAccount);

                symbolsCache = symbolsCacheFactory.GetSymbolsCache(accountLogin.Exchange);

                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(DispatcherTimerTick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
                dispatcherTimer.Start();

                DispatcherTimerTick(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                OnException("AccountViewModel.Login", ex);
            }

            IsLoggingIn = false;
        }

        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            try
            {
                lock (balancesLock)
                {
                    symbolsCache.ValueAccount(Account);
                }
            }
            catch(Exception ex)
            {
                OnException("AccountViewModel.DispatcherTimerTick", ex);
            }
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

        private void AccountNotification(AccountEventType accountEventType)
        {
            var onAccountNotification = OnAccountNotification;
            onAccountNotification?.Invoke(this, new AccountEventArgs { Value = Account, AccountEventType = accountEventType });
        }

        private void AccountInfoUpdate(TradeView.Interface.Model.AccountInfo e)
        {
            Action<TradeView.Interface.Model.AccountInfo> action = aie =>
            {
                lock (balancesLock)
                {
                    if (aie.Balances == null
                        || !aie.Balances.Any())
                    {
                        Account.Balances.Clear();
                        return;
                    }

                    Func<AccountBalance, TradeView.Interface.Model.AccountBalance, AccountBalance> f = ((ab, nb) =>
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

            AccountNotification(AccountEventType.UpdateOrders);
        }
    }
}
