using DevelopmentInProgress.MarketView.Interface.Extensions;
using DevelopmentInProgress.Wpf.Host.ViewModel;
using DevelopmentInProgress.Wpf.MarketView.Events;
using DevelopmentInProgress.Wpf.MarketView.Extensions;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using InterfaceExtensions = DevelopmentInProgress.MarketView.Interface.Extensions;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class TradeViewModel : BaseViewModel
    {
        private List<Symbol> symbols;
        private Symbol selectedSymbol;
        private Account account;
        private AccountBalance baseAccountBalance;
        private AccountBalance quoteAccountBalance;
        private string selectedOrderType;
        private decimal quantity;
        private decimal price;
        private decimal stopPrice;
        private bool disposed;
        private bool isLoading;

        public TradeViewModel(IExchangeService exchangeService)
            : base(exchangeService)
        {
            BuyCommand = new ViewModelCommand(Buy);
            SellCommand = new ViewModelCommand(Sell);
            BuyQuantityCommand = new ViewModelCommand(BuyQuantity);
            SellQuantityCommand = new ViewModelCommand(SellQuantity);
            OnPropertyChanged("");
        }

        public event EventHandler<TradeEventArgs> OnTradeNotification;
        
        public ICommand BuyCommand { get; set; }
        public ICommand SellCommand { get; set; }
        public ICommand BuyQuantityCommand { get; set; }
        public ICommand SellQuantityCommand { get; set; }

        public Account Account
        {
            get { return account; }
            set
            {
                if (account != value)
                {
                    account = value;
                }
            }
        }

        public AccountBalance BaseAccountBalance
        {
            get { return baseAccountBalance; }
            set
            {
                if (baseAccountBalance != value)
                {
                    baseAccountBalance = value;
                    OnPropertyChanged("HasBaseBalance");
                    OnPropertyChanged("BaseAccountBalance");
                }
            }
        }

        public AccountBalance QuoteAccountBalance
        {
            get { return quoteAccountBalance; }
            set
            {
                if (quoteAccountBalance != value)
                {
                    quoteAccountBalance = value;
                    OnPropertyChanged("HasQuoteBalance");
                    OnPropertyChanged("QuoteAccountBalance");
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

        public List<Symbol> Symbols
        {
            get { return symbols; }
            set
            {
                if (symbols != value)
                {
                    symbols = value;
                    OnPropertyChanged("Symbols");
                }
            }
        }

        public Symbol SelectedSymbol
        {
            get { return selectedSymbol; }
            set
            {
                if (selectedSymbol != value)
                {
                    selectedSymbol = value;
                    
                    if(selectedSymbol != null)
                    {
                        Quantity = 0;
                        Price = SelectedSymbol.SymbolStatistics.LastPrice;
                        StopPrice = SelectedSymbol.SymbolStatistics.LastPrice;
                        BaseAccountBalance = Account?.Balances.SingleOrDefault(ab => ab.Asset.Equals(selectedSymbol.BaseAsset.Symbol));
                        QuoteAccountBalance = Account?.Balances.SingleOrDefault(ab => ab.Asset.Equals(selectedSymbol.QuoteAsset.Symbol));
                    }
                    else
                    {
                        Price = 0;
                        StopPrice = 0;
                        Quantity = 0;
                        BaseAccountBalance = null;
                        QuoteAccountBalance = null;                        
                    }

                    OnPropertyChanged("OrderTypes");
                    OnPropertyChanged("SelectedSymbol");
                }
            }
        }

        public decimal Quantity
        {
            get { return quantity; }
            set
            {
                if (quantity != value)
                {
                    if (SelectedSymbol != null
                        && value.HasRemainder())
                    {
                        quantity = value.Trim(SelectedSymbol.QuantityPrecision);
                    }
                    else
                    {
                        quantity = value;
                    }

                    OnPropertyChanged("Quantity");
                }
            }
        }

        public decimal Price
        {
            get { return price; }
            set
            {
                if (price != value)
                {
                    if (SelectedSymbol != null
                        && value.HasRemainder())
                    {
                        price = value.Trim(SelectedSymbol.PricePrecision);
                    }
                    else
                    {
                        price = value;
                    }

                    OnPropertyChanged("Price");
                }
            }
        }

        public decimal StopPrice
        {
            get { return stopPrice; }
            set
            {
                if (stopPrice != value)
                {
                    if (SelectedSymbol != null
                        && value.HasRemainder())
                    {
                        stopPrice = value.Trim(SelectedSymbol.PricePrecision);
                    }
                    else
                    {
                        stopPrice = value;
                    }

                    OnPropertyChanged("StopPrice");
                }
            }
        }

        public string SelectedOrderType
        {
            get { return selectedOrderType; }
            set
            {
                if (selectedOrderType != value)
                {
                    selectedOrderType = value;

                    if (SelectedSymbol != null)
                    {
                        Price = SelectedSymbol.SymbolStatistics.LastPrice;
                        StopPrice = SelectedSymbol.SymbolStatistics.LastPrice;
                    }

                    OnPropertyChanged("IsPriceEditable");
                    OnPropertyChanged("IsMarketPrice");
                    OnPropertyChanged("IsStopLoss");
                    OnPropertyChanged("SelectedOrderType");
                }
            }
        }

        public bool IsPriceEditable
        {
            get
            {
                if (IsLoading)
                {
                    return !IsLoading;
                }

                return !SelectedOrderType.IsMarketOrder();
            }
        }

        public bool IsMarketPrice
        {
            get
            {
                if (IsLoading)
                {
                    return !IsLoading;
                }

                return SelectedOrderType.IsMarketOrder();
            }
        }

        public bool IsStopLoss
        {
            get
            {
                if (IsLoading)
                {
                    return !IsLoading;
                }

                return SelectedOrderType.IsStopLoss();
            }
        }

        public bool HasBaseBalance
        {
            get
            {
                if (BaseAccountBalance != null
                    && BaseAccountBalance.Free > 0)
                {
                    return true;
                }

                return false;
            }
        }

        public bool HasQuoteBalance
        {
            get
            {
                if (QuoteAccountBalance != null
                    && QuoteAccountBalance.Free > 0)
                {
                    return true;
                }

                return false;
            }
        }

        public string[] OrderTypes
        {
            get
            {
                if(SelectedSymbol != null)
                {
                    return SelectedSymbol.OrderTypes.GetOrderTypeNames();
                }

                return InterfaceExtensions.OrderExtensions.OrderTypes();
            }
        }
        
        public void SetSymbols(List<Symbol> symbols)
        {
            try
            {
                Symbols = symbols;
            }
            catch (Exception e)
            {
                OnException("TradeViewModel.SetSymbols", e);
            }
        }

        public void SetAccount(Account account, AccountBalance selectedAsset)
        {
            try
            {
                if (Account == null
                    || !Account.ApiKey.Equals(account.ApiKey))
                {
                    Account = account;
                    SelectedOrderType = string.Empty;
                }

                if (selectedAsset == null)
                {
                    SelectedSymbol = null;
                }
                else
                {
                    SelectedSymbol = Symbols.FirstOrDefault(s => s.BaseAsset.Symbol.Equals(selectedAsset.Asset));
                }
            }
            catch (Exception e)
            {
                OnException("TradeViewModel.SetAccount", e);
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
            }

            disposed = true;
        }

        private void Buy(object param)
        {
            SendClientOrder(Interface.OrderSide.Buy);
        }

        private void Sell(object param)
        {
            SendClientOrder(Interface.OrderSide.Sell);
        }

        private async void SendClientOrder(Interface.OrderSide orderSide)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(selectedOrderType))
                {
                    throw new Exception("Order not valid: No order type.");
                }

                var clientOrder = new Interface.ClientOrder
                {
                    Symbol = SelectedSymbol?.Name,
                    Type = SelectedOrderType.GetOrderType(),
                    Side = orderSide,
                    Quantity = Quantity,
                    Price = Price,
                    StopPrice = StopPrice,
                    BaseAccountBalance = BaseAccountBalance.GetInterfaceAccountBalance(),
                    QuoteAccountBalance = QuoteAccountBalance.GetInterfaceAccountBalance()
                };

                SelectedSymbol.GetInterfaceSymbol().ValidateClientOrder(clientOrder);

                await ExchangeService.PlaceOrder(Account.AccountInfo.User, clientOrder).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                OnException("TradeViewModel.SendClientOrder", e);
            }
        }

        private void BuyQuantity(object param)
        {
            if (Price != 0)
            {
                var qty = QuoteAccountBalance.Free / Price;
                SetQuantity(param.ToString(), qty);
            }
        }

        private void SellQuantity(object param)
        {
            SetQuantity(param.ToString(), BaseAccountBalance.Free);
        }

        private void SetQuantity(string percentage, decimal total)
        {
            decimal percent;
            if (Decimal.TryParse(percentage, out percent))
            {
                Quantity = (percent / 100) * total;
            }
        }

        private void OnException(string message, Exception exception)
        {
            var onTradeNotification = OnTradeNotification;
            onTradeNotification?.Invoke(this, new TradeEventArgs { Message = message, Exception = exception });
        }
    }
}
