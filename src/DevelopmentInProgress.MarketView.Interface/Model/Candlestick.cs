using System;

namespace DevelopmentInProgress.MarketView.Interface.Model
{
    public class Candlestick
    {
        public string Symbol { get; }
        public CandlestickInterval Interval { get; }
        public DateTime OpenTime { get; }
        public decimal Open { get; }
        public decimal High { get; }
        public decimal Low { get; }
        public decimal Close { get; }
        public decimal Volume { get; }
        public DateTime CloseTime { get; }
        public decimal QuoteAssetVolume { get; }
        public long NumberOfTrades { get; }
        public decimal TakerBuyBaseAssetVolume { get; }
        public decimal TakerBuyQuoteAssetVolume { get; }
    }
}