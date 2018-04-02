using Interface = DevelopmentInProgress.MarketView.Interface.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.MarketView.Model
{
    public class Symbol : EntityBase
    {
        private AccountBalance accountBalance;
        private int lastPriceChangeDirection;
        private int priceChangePercentDirection;

        public decimal NotionalMinimumValue { get; set; }
        public Interface.Asset BaseAsset { get; set; }
        public Interface.InclusiveRange Price { get; set; }
        public Interface.InclusiveRange Quantity { get; set; }
        public Interface.Asset QuoteAsset { get; set; }
        public Interface.SymbolStatus Status { get; set; }
        public bool IsIcebergAllowed { get; set; }
        public IEnumerable<Interface.OrderType> OrderTypes { get; set; }
        public SymbolStatistics SymbolStatistics { get; set; }
        public bool IsFavourite { get; set; }

        public string Name { get { return $"{BaseAsset.Symbol}{QuoteAsset.Symbol}"; } }

        public AccountBalance AccountBalance
        {
            get { return accountBalance; }
            set
            {
                if (accountBalance != value)
                {
                    accountBalance = value;
                    OnPropertyChanged("HasBalance");
                    OnPropertyChanged("AccountBalance");
                }
            }
        }

        public int LastPriceChangeDirection
        {
            get { return lastPriceChangeDirection; }
            set
            {
                if (lastPriceChangeDirection != value)
                {
                    lastPriceChangeDirection = value;
                    OnPropertyChanged("LastPriceChangeDirection");
                }
            }
        }

        public int PriceChangePercentDirection
        {
            get { return priceChangePercentDirection; }
            set
            {
                if (priceChangePercentDirection != value)
                {
                    priceChangePercentDirection = value;
                    OnPropertyChanged("PriceChangePercentDirection");
                }
            }
        }

        public bool HasBalance
        {
            get
            {
                if (AccountBalance != null
                && AccountBalance.Free > 0)
                {
                    return true;
                }

                return false;
            }
        }

        public override string ToString()
        {
            return $"{BaseAsset.Symbol} / {QuoteAsset.Symbol}"; ;
        }
    }
}
