using DevelopmentInProgress.Wpf.Common.Model;

namespace DevelopmentInProgress.Wpf.Common.Extensions
{
    public static class CandlestickExtensions
    {
        public static Candlestick ToViewCandlestick(this MarketView.Interface.Model.Candlestick c)
        {
            return new Candlestick
            {
                Symbol = c.Symbol,
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
