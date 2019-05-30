using System;
using System.Collections.Generic;
using System.Text;

namespace DevelopmentInProgress.MarketView.Interface.Strategy
{
    public class StrategyPerformanceIndicator
    {
        public string Symbol { get; set; }
        public string BaseAsset { get; set; }
        public decimal BaseQuantity { get; set; }
        public string QuoteAsset { get; set; }
        public decimal QuoteQuantity { get; set; }
        public decimal IndicativePrice { get; set; }
        public DateTime IndicationPriceTime { get; set; }
    }
}
