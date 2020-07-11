using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class CandlestickExtensions
    {
        public static Candlestick ToViewCandlestick(this Core.Model.Candlestick c)
        {
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }

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
