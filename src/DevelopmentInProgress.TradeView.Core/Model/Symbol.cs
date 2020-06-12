using DevelopmentInProgress.TradeView.Core.Enums;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Core.Model
{
    public class Symbol
    {
        public string Name { get; set; }
        public Exchange Exchange { get; set; }
        public string NameDelimiter { get; set; }
        public string ExchangeSymbol { get; set; }
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
