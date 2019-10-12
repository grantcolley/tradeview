using DevelopmentInProgress.Common.Extensions;
using DevelopmentInProgress.Wpf.Common.Command;
using DevelopmentInProgress.Wpf.Common.Extensions;
using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.Common.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using DevelopmentInProgress.Wpf.Common.Events;
using Prism.Logging;

namespace DevelopmentInProgress.Wpf.Common.ViewModel
{
    public class OrdersViewModel : ExchangeViewModel
    {
        private CancellationTokenSource ordersCancellationTokenSource;
        private Account account;
        private ObservableCollection<Order> orders;
        private Order selectedOrder;
        private bool isLoading;
        private bool isCancellAllVisible;
        private bool disposed;

        private object lockOrders = new object();

        public OrdersViewModel(IWpfExchangeService exchangeService, ILoggerFacade logger)
            : base(exchangeService, logger)
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

        public async Task SetAccount(Account account)
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
                        var result = await Task.Run(async () => await ExchangeService.GetOpenOrdersAsync(Account.AccountInfo.User.Exchange, Account.AccountInfo.User));

                        lock(lockOrders)
                        {
                            Orders.Clear();
                            foreach (var order in result)
                            {
                                Orders.Add(order);
                            }
                        }
                    }
                    else
                    {
                        lock (lockOrders)
                        {
                            Orders.Clear();
                        }

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
                OnException("OrdersViewModel.SetAccount", ex);
            }

            IsLoading = false;
        }

        public async Task UpdateOrders(Account acccount)
        {
            try
            {
                var result = await Task.Run(async () => await ExchangeService.GetOpenOrdersAsync(Account.AccountInfo.User.Exchange, Account.AccountInfo.User));

                Action<IEnumerable<Order>> action = res =>
                {
                    lock (lockOrders)
                    {
                        if (!res.Any())
                        {
                            Orders.Clear();
                            return;
                        }

                        var updated = (from o in Orders
                                       join r in res on o.ClientOrderId equals r.ClientOrderId
                                       select o.Update(r)).ToList();

                        var remove = Orders.Where(o => !res.Any(r => r.ClientOrderId.Equals(o.ClientOrderId)));
                        foreach (var order in remove)
                        {
                            Orders.Remove(order);
                        }

                        var add = res.Where(r => !Orders.Any(o => o.ClientOrderId.Equals(r.ClientOrderId)));
                        foreach (var order in add)
                        {
                            Orders.Add(order);
                        }
                    }
                };

                if (Dispatcher == null)
                {
                    action(result);
                }
                else
                {
                    Dispatcher.Invoke(action, result);
                }
            }
            catch (Exception ex)
            {
                OnException("OrdersViewModel.UpdateOrders", ex);
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

        private void Cancel(object param)
        {
            try
            {
                var orderId = long.Parse(param.ToString());
                Cancel(orderId.ToString()).FireAndForget();
            }
            catch (Exception ex)
            {
                OnException("OrdersViewModel.Cancel", ex);
            }
        }

        private async Task Cancel(string orderId)
        {
            try
            {
                var order = orders.Single(o => o.Id == orderId);
                order.IsVisible = false;
                var result = await ExchangeService.CancelOrderAsync(Account.AccountInfo.User.Exchange, Account.AccountInfo.User, order.Symbol, order.Id, null, 0, ordersCancellationTokenSource.Token);
                lock (lockOrders)
                {
                    Orders.Remove(order);
                }
            }
            catch (Exception ex)
            {
                OnException("OrdersViewModel.Cancel", ex);
            }
        }

        private void CancelAll(object param)
        {
            try
            {
                IsCancellAllVisible = false;
                Parallel.ForEach(Orders, async order => { var result = await ExchangeService.CancelOrderAsync(Account.AccountInfo.User.Exchange, Account.AccountInfo.User, order.Symbol, order.Id, null, 0, ordersCancellationTokenSource.Token); });
                lock (lockOrders)
                {
                    Orders.Clear();
                }

                IsCancellAllVisible = true;
            }
            catch (Exception ex)
            {
                OnException("OrdersViewModel.CancelAll", ex);
            }
        }

        private void OnException(string message, Exception exception)
        {
            var onOrdersNotification = OnOrdersNotification;
            onOrdersNotification?.Invoke(this, new OrdersEventArgs { Message = message, Exception = exception });
        }

        private void OrdersNotification(List<Order> orders)
        {
            var onOrdersNotification = OnOrdersNotification;
            onOrdersNotification?.Invoke(this, new OrdersEventArgs { Value = orders });
        }
    }
}
