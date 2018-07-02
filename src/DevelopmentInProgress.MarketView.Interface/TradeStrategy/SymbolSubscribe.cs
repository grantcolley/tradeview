namespace DevelopmentInProgress.MarketView.Interface.TradeStrategy
{
    public class SymbolSubscribe
    {
        public string Symbol { get; set; }
        public Exchange Exchange { get; set; }
        public Subscribe Subscribe { get; set; }
    }
}