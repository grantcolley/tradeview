using DevelopmentInProgress.TradeView.Wpf.Common.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class CandlestickExtensions
    {
        public static Candlestick ToViewCandlestick(this Interface.Model.Candlestick c)
        {
            return new Candlestick
            {
                Symbol = c.Symbol,
                Exchange = c.Exchange,
                Interval = c.Interval,
                OpenTime = c.OpenTime.ToLocalTime(),
                Open = c.Open,
                High = c.High,
                Low = c.Low,
                Close = c.Close,
                CloseTime = c.CloseTime.ToLocalTime(),
                Volume = c.Volume,
                QuoteAssetVolume = c.QuoteAssetVolume,
                NumberOfTrades = c.NumberOfTrades,
                TakerBuyBaseAssetVolume = c.TakerBuyBaseAssetVolume,
                TakerBuyQuoteAssetVolume = c.TakerBuyQuoteAssetVolume
            };
        }
    }
}
