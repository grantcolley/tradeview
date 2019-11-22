using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class SymbolStatisticsExtensions
    {
        public static SymbolStatistics GetViewSymbolStatistics(this Interface.Model.SymbolStats s)
        {
            return new SymbolStatistics
            {
                FirstTradeId = s.FirstTradeId,
                CloseTime = s.CloseTime,
                OpenTime = s.OpenTime,
                QuoteVolume = s.QuoteVolume,
                Volume = Convert.ToInt64(s.Volume),
                LowPrice = s.LowPrice,
                HighPrice = s.HighPrice,
                OpenPrice = s.OpenPrice,
                AskQuantity = s.AskQuantity,
                AskPrice = s.AskPrice,
                BidQuantity = s.BidQuantity,
                BidPrice = s.BidPrice,
                LastQuantity = s.LastQuantity,
                LastPrice = s.LastPrice,
                PreviousClosePrice = s.PreviousClosePrice,
                WeightedAveragePrice = s.WeightedAveragePrice,
                PriceChangePercent = decimal.Round(s.PriceChangePercent, 2, MidpointRounding.AwayFromZero),
                PriceChange = s.PriceChange,
                Period = s.Period,
                Symbol = s.Symbol,
                LastTradeId = s.LastTradeId,
                TradeCount = s.TradeCount
            };
        }
    }
}