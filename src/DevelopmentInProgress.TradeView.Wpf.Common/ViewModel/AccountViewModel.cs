using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using Prism.Logging;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DevelopmentInProgress.TradeView.Wpf.Common.ViewModel
{
    public class AccountViewModel : BaseViewModel
    {
        private AccountBalancesViewModel accountBalancesViewModel;
        private OrdersViewModel ordersViewModel;
        private Account account;
        private bool disposed;

        private IDisposable accountObservableSubscription;
        private IDisposable ordersObservableSubscription;

        public AccountViewModel(
            AccountBalancesViewModel accountBalancesViewModel,
            OrdersViewModel ordersViewModel, 
            ILoggerFacade logger)
            : base(logger)
        {
            this.accountBalancesViewModel = accountBalancesViewModel;
            this.ordersViewModel = ordersViewModel;

            ObserveAccountBalances();
            ObserveOrders();
        }

        public event EventHandler<AccountEventArgs> OnAccountNotification;

        public override Dispatcher Dispatcher
        {
            get { return base.Dispatcher; }
            set
            {
                base.Dispatcher = value;
                accountBalancesViewModel.Dispatcher = value;
                ordersViewModel.Dispatcher = value;
            }
        }

        public AccountBalancesViewModel AccountBalancesViewModel
        {
            get { return accountBalancesViewModel; }
            set
            {
                if(accountBalancesViewModel != value)
                {
                    accountBalancesViewModel = value;
                    OnPropertyChanged(nameof(AccountBalancesViewModel));
                }
            }
        }

        public OrdersViewModel OrdersViewModel
        {
            get { return ordersViewModel; }
            set
            {
                if (ordersViewModel != value)
                {
                    ordersViewModel = value;
                    OnPropertyChanged(nameof(OrdersViewModel));
                }
            }
        }

        public void SetAccount(Account account)
        {
            this.account = account;
        }

        public Task Login(Account account = null)
        {
            if(account == null)
            {
                if(this.account == null)
                {
                    throw new ArgumentNullException(nameof(account));
                }

                account = this.account;
            }

            return accountBalancesViewModel.Login(account);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                accountBalancesViewModel.Dispose();
                ordersViewModel.Dispose();

                ordersObservableSubscription?.Dispose();
                accountObservableSubscription.Dispose();
            }

            disposed = true;
        }

        private void ObserveAccountBalances()
        {
            var accountObservable = Observable.FromEventPattern<AccountEventArgs>(
                eventHandler => AccountBalancesViewModel.OnAccountNotification += eventHandler,
                eventHandler => AccountBalancesViewModel.OnAccountNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            accountObservableSubscription = accountObservable.Subscribe(async args =>
            {
                if (args.HasException)
                {
                    AccountNotification(args);
                }
                else if (args.AccountEventType.Equals(AccountEventType.LoggedIn))
                {
                    AccountNotification(args);
                    await OrdersViewModel.SetAccount(args.Value).ConfigureAwait(false);
                }
                else if (args.AccountEventType.Equals(AccountEventType.UpdateOrders))
                {
                    AccountNotification(args);
                    await OrdersViewModel.UpdateOrders(args.Value).ConfigureAwait(false);
                }
                else if (args.AccountEventType.Equals(AccountEventType.SelectedAsset))
                {
                    AccountNotification(args);
                }
            });
        }

        private void ObserveOrders()
        {
            var ordersObservable = Observable.FromEventPattern<OrdersEventArgs>(
                eventHandler => OrdersViewModel.OnOrdersNotification += eventHandler,
                eventHandler => OrdersViewModel.OnOrdersNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            ordersObservableSubscription = ordersObservable.Subscribe(args =>
            {
                if (args.HasException)
                {
                    AccountNotification(new AccountEventArgs
                    {
                        AccountEventType = AccountEventType.OrdersNotification,
                        Message = args.Message,
                        Exception = args.Exception
                    }
                    );
                }
            });
        }

        private void AccountNotification(AccountEventArgs args)
        {
            var onAccountNotification = OnAccountNotification;
            onAccountNotification?.Invoke(this, args);
        }
    }
}
