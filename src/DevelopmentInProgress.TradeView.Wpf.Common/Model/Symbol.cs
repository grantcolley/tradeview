using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Extensions;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class Symbol : EntityBase
    {
        private int lastPriceChangeDirection;
        private int priceChangePercentDirection;
        private bool isFavourite;

        public string Name { get; set; }
        public Exchange Exchange { get; set; }
        public string NameDelimiter { get; set; }
        public string ExchangeSymbol { get; set; }
        public decimal NotionalMinimumValue { get; set; }
        public Core.Model.Asset BaseAsset { get; set; }
        public Core.Model.InclusiveRange Price { get; set; }
        public Core.Model.InclusiveRange Quantity { get; set; }
        public Core.Model.Asset QuoteAsset { get; set; }
        public Core.Model.SymbolStatus Status { get; set; }
        public bool IsIcebergAllowed { get; set; }
        public IEnumerable<Core.Model.OrderType> OrderTypes { get; set; }
        public SymbolStatistics SymbolStatistics { get; set; }

        public bool IsFavourite 
        {
            get { return isFavourite; }
            set
            {
                if(isFavourite != value)
                {
                    isFavourite = value;
                    OnPropertyChanged(nameof(IsFavourite));
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
                    OnPropertyChanged(nameof(LastPriceChangeDirection));
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
                    OnPropertyChanged(nameof(PriceChangePercentDirection));
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
