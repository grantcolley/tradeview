using DevelopmentInProgress.Wpf.MarketView.Events;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class OrdersViewModel : BaseViewModel
    {
        private CancellationTokenSource ordersCancellationTokenSource;
        private Account account;
        private List<Order> orders;
        private Order selectedOrder;
        private bool isLoading;
        private bool disposed;

        public OrdersViewModel(IExchangeService exchangeService)
            : base(exchangeService)
        {
            ordersCancellationTokenSource = new CancellationTokenSource();
        }

        public event EventHandler<OrdersEventArgs> OnOrdersNotification;

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

        public List<Order> Orders
        {
            get { return orders; }
            set
            {
                if (orders != value)
                {
                    orders = value;
                    OnPropertyChanged("Orders");
                }
            }
        }

        public Order SelectedOrder
        {
            get { return selectedOrder; }
            set
            {
                if(selectedOrder != value)
                {
                    selectedOrder = value;
                    OnPropertyChanged("SelectedOrder");
                }
            }
        }

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged("IsLoading");
                }
            }
        }

        public async void SetAccount(Account account)
        {
            Account = account;
            var orders = await Task.Run(async () => await ExchangeService.GetOpenOrdersAsync(Account.AccountInfo.User));
            Orders = new List<Order>(orders);
        }

        public override void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (ordersCancellationTokenSource != null
                    && !ordersCancellationTokenSource.IsCancellationRequested)
                {
                    ordersCancellationTokenSource.Cancel();
                }
            }

            disposed = true;
        }

        private void OnException(Exception exception)
        {
            var onOrdersNotification = OnOrdersNotification;
            onOrdersNotification?.Invoke(this, new OrdersEventArgs { Exception = exception });
        }

        private void OrdersNotification(List<Order> orders)
        {
            var onOrdersNotification = OnOrdersNotification;
            onOrdersNotification?.Invoke(this, new OrdersEventArgs { Value = orders });
        }
    }
}
