using DevelopmentInProgress.TradeView.Interface.Extensions;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class Symbol : EntityBase
    {
        private int lastPriceChangeDirection;
        private int priceChangePercentDirection;
        private bool isFavourite;

        public string ExchangeSymbol { get; set; }
        public decimal NotionalMinimumValue { get; set; }
        public Interface.Model.Asset BaseAsset { get; set; }
        public Interface.Model.InclusiveRange Price { get; set; }
        public Interface.Model.InclusiveRange Quantity { get; set; }
        public Interface.Model.Asset QuoteAsset { get; set; }
        public Interface.Model.SymbolStatus Status { get; set; }
        public bool IsIcebergAllowed { get; set; }
        public IEnumerable<Interface.Model.OrderType> OrderTypes { get; set; }
        public SymbolStatistics SymbolStatistics { get; set; }

        public string Name { get { return $"{BaseAsset.Symbol}{QuoteAsset.Symbol}"; } }

        public bool IsFavourite 
        {
            get { return isFavourite; }
            set
            {
                if(isFavourite != value)
                {
                    isFavourite = value;
                    OnPropertyChanged("IsFavourite");
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

        public int QuantityPrecision
        {
            get
            {
                return Quantity.Increment.GetPrecision();
            }
        }

        public int PricePrecision
        {
            get
            {
                return Price.Increment.GetPrecision();
            }
        }

        public override string ToString()
        {
            return $"{BaseAsset.Symbol} / {QuoteAsset.Symbol}";
        }
    }
}
