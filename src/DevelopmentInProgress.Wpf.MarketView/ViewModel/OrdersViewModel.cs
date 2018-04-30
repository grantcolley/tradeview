using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.MarketView.Events;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class OrdersViewModel : BaseViewModel
    {
        private CancellationTokenSource ordersCancellationTokenSource;
        private Account account;
        private ObservableCollection<Order> orders;
        private Order selectedOrder;
        private bool isLoading;
        private bool isCancellAllVisible;
        private bool disposed;

        public OrdersViewModel(IExchangeService exchangeService)
            : base(exchangeService)
        {
            CancelOrderCommand = new ViewModelCommand(Cancel);
            CancelAllOrdersCommand = new ViewModelCommand(CancelAll);

            ordersCancellationTokenSource = new CancellationTokenSource();

            Orders = new ObservableCollection<Order>();

            IsCancellAllVisible = true;
        }

        public event EventHandler<OrdersEventArgs> OnOrdersNotification;

        public ICommand CancelOrderCommand { get; set; }
        public ICommand CancelAllOrdersCommand { get; set; }

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

        public ObservableCollection<Order> Orders
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

        public bool IsCancellAllVisible
        {
            get { return isCancellAllVisible; }
            set
            {
                if (isCancellAllVisible != value)
                {
                    isCancellAllVisible = value;
                    OnPropertyChanged("IsCancellAllVisible");
                }
            }
        }

        public async void SetAccount(Account account)
        {
            IsLoading = true;

            try
            {
                if (Account == null
                    || account == null
                    || !Account.ApiKey.Equals(account.ApiKey))
                {
                    Account = account;

                    if (Account != null)
                    {
                        Orders.Clear();
                        var result = await Task.Run(async () => await ExchangeService.GetOpenOrdersAsync(Account.AccountInfo.User));
                        foreach (var order in result)
                        {
                            Orders.Add(order);
                        }
                    }
                    else
                    {
                        Orders.Clear();
                        if (ordersCancellationTokenSource != null
                            && !ordersCancellationTokenSource.IsCancellationRequested)
                        {
                            ordersCancellationTokenSource.Cancel();
                        }

                        ordersCancellationTokenSource = new CancellationTokenSource();
                    }
                }
            }
            catch (Exception ex)
            {
                OnException(ex);
            }

            IsLoading = false;
        }

        public async void UpdateOrders(Account acccount)
        {
            try
            {
                var result = await Task.Run(async () => await ExchangeService.GetOpenOrdersAsync(Account.AccountInfo.User));

                Dispatcher.Invoke(() =>
                {
                    if (!result.Any())
                    {
                        Orders.Clear();
                        return;
                    }

                    var updated = (from o in Orders
                                   join r in result on o.Symbol equals r.Symbol
                                   select o.Update(r)).ToList();

                    var remove = Orders.Where(o => !result.Any(r => r.Symbol.Equals(o.Symbol)));
                    foreach (var order in remove)
                    {
                        Orders.Remove(order);
                    }

                    var add = result.Where(r => !Orders.Any(o => o.Symbol.Equals(r.Symbol)));
                    foreach (var order in add)
                    {
                        Orders.Add(order);
                    }
                });
            }
            catch (Exception ex)
            {
                OnException(ex);
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
                if (ordersCancellationTokenSource != null
                    && !ordersCancellationTokenSource.IsCancellationRequested)
                {
                    ordersCancellationTokenSource.Cancel();
                }
            }

            disposed = true;
        }

        private async void Cancel(object param)
        {
            try
            {
                var orderId = long.Parse(param.ToString());
                var order = orders.Single(o => o.Id == orderId);
                order.IsVisible = false;
                var result = await ExchangeService.CancelOrderAsync(Account.AccountInfo.User, order.Symbol, order.Id, null, 0, ordersCancellationTokenSource.Token);
                Orders.Remove(order);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        private void CancelAll(object param)
        {
            try
            {
                IsCancellAllVisible = false;
                Parallel.ForEach(Orders, async order => { var result = await ExchangeService.CancelOrderAsync(Account.AccountInfo.User, order.Symbol, order.Id, null, 0, ordersCancellationTokenSource.Token); });
                Orders.Clear();
                IsCancellAllVisible = true;
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
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
