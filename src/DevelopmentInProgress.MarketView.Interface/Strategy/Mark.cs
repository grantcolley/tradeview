using System;

namespace DevelopmentInProgress.MarketView.Interface.Strategy
{
    public class Mark
    {
        public string BaseAsset { get; set; }
        public decimal BaseQuantity { get; set; }
        public string QuoteAsset { get; set; }
        public decimal QuoteQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal MarkValue { get; set; }
        public DateTime MarkTime { get; set; }
        public decimal Performance { get; set; }

        public Mark Clone()
        {
            return MemberwiseClone() as Mark;
        }
    }
}