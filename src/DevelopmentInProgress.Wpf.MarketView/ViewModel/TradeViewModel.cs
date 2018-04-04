using DevelopmentInProgress.MarketView.Interface.Helpers;
using DevelopmentInProgress.Wpf.MarketView.Events;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

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
        private bool disposed;
        private bool isLoading;

        public TradeViewModel(IExchangeService exchangeService)
            : base(exchangeService)
        {
        }

        public event EventHandler<TradeEventArgs> OnTradeNotification;

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
                        BaseAccountBalance = Account?.Balances.SingleOrDefault(ab => ab.Asset.Equals(selectedSymbol.BaseAsset.Symbol));
                        QuoteAccountBalance = Account?.Balances.SingleOrDefault(ab => ab.Asset.Equals(selectedSymbol.QuoteAsset.Symbol));
                    }

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
                    quantity = value;
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
                    price = value;
                    OnPropertyChanged("Price");
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
                    }

                    OnPropertyChanged("IsPriceEditable");
                    OnPropertyChanged("IsMarketPrice");
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

                return !OrderTypeHelper.AreEqual(Interface.OrderType.Market, SelectedOrderType);
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

                return OrderTypeHelper.AreEqual(Interface.OrderType.Market, SelectedOrderType);
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
            get { return OrderTypeHelper.OrderTypes(); }
        }
        
        public void SetSymbols(List<Symbol> symbols)
        {
            Symbols = symbols;
        }

        public void SetAccount(Account account, AccountBalance selectedAsset)
        {
            Account = account;
            if (selectedAsset != null)
            {
                SelectedSymbol = Symbols.FirstOrDefault(s => s.BaseAsset.Symbol.Equals(selectedAsset.Asset));
            }

            SelectedOrderType = OrderTypeHelper.GetOrderTypeName(Interface.OrderType.Limit);
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

        private void OnException(Exception exception)
        {
            var onTradeNotification = OnTradeNotification;
            onTradeNotification?.Invoke(this, new TradeEventArgs { Exception = exception });
        }
    }
}
