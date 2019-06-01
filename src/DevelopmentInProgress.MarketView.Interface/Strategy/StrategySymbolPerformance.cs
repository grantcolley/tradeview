using DevelopmentInProgress.MarketView.Interface.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.Strategy
{
    public class StrategySymbolPerformance
    {
        public StrategySymbolPerformance()
        {
            Trades = new List<AccountTrade>();
        }

        public string Symbol { get; set; }
        public string BaseAsset { get; set; }
        public decimal BaseQuantity { get; set; }
        public string QuoteAsset { get; set; }
        public decimal QuoteQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal Value { get; set; }
        public List<AccountTrade> Trades { get; set; }
    }
}
