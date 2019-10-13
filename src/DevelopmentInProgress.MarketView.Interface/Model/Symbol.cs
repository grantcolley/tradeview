using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.Model
{
    public class Symbol
    {
        public decimal NotionalMinimumValue { get; set; }
        public Asset BaseAsset { get; set; }
        public InclusiveRange Price { get; set; }
        public InclusiveRange Quantity { get; set; }
        public Asset QuoteAsset { get; set; }
        public SymbolStatus Status { get; set; }
        public bool IsIcebergAllowed { get; set; }
        public IEnumerable<OrderType> OrderTypes { get; set; }
        public SymbolStats SymbolStatistics { get; set; }
    }
}
