namespace DevelopmentInProgress.Wpf.Common.Model
{
    public class Trade : TradeBase
    {
        public string Symbol { get; set; }
        public bool IsBestPriceMatch { get; set; }
    }
}