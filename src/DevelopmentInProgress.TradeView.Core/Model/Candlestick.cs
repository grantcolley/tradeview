using DevelopmentInProgress.TradeView.Core.Enums;
using System;

namespace DevelopmentInProgress.TradeView.Core.Model
{
    public class Candlestick
    {
        public string Symbol { get; set; }
        public Exchange Exchange { get; set; }
        public CandlestickInterval Interval { get; set; }
        public DateTime OpenTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public DateTime CloseTime { get; set; }
        public decimal QuoteAssetVolume { get; set; }
        public long NumberOfTrades { get; set; }
        public decimal TakerBuyBaseAssetVolume { get; set; }
        public decimal TakerBuyQuoteAssetVolume { get; set; }
    }
}