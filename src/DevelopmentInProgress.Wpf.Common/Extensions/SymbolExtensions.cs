using DevelopmentInProgress.MarketView.Interface.Extensions;
using DevelopmentInProgress.Wpf.Common.Model;
using System;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.Common.Extensions
{
    public static class SymbolExtensions
    {
        public static Interface.Symbol GetInterfaceSymbol(this Symbol s)
        {
            return new Interface.Symbol
            {
                NotionalMinimumValue = s.NotionalMinimumValue,
                BaseAsset = s.BaseAsset,
                Price = s.Price,
                Quantity = s.Quantity,
                QuoteAsset = s.QuoteAsset,
                Status = s.Status,
                IsIcebergAllowed = s.IsIcebergAllowed,
                OrderTypes = s.OrderTypes
            };
        }

        public static Symbol GetViewSymbol(this Interface.Symbol s)
        {
            return new Symbol
            {
                NotionalMinimumValue = s.NotionalMinimumValue,
                BaseAsset = s.BaseAsset,
                Price = s.Price,
                Quantity = s.Quantity,
                QuoteAsset = s.QuoteAsset,
                Status = s.Status,
                IsIcebergAllowed = s.IsIcebergAllowed,
                OrderTypes = s.OrderTypes,
                SymbolStatistics = new SymbolStatistics { Symbol = $"{s.BaseAsset.Symbol}{s.QuoteAsset.Symbol}" }
            };
        }

        public static Symbol UpdateStatistics(this Symbol sy, Interface.SymbolStats st)
        {
            sy.SymbolStatistics.PriceChangePercent = decimal.Round(st.PriceChangePercent, 2, MidpointRounding.AwayFromZero);
            sy.PriceChangePercentDirection = sy.SymbolStatistics.PriceChangePercent > 0 ? 1 : sy.SymbolStatistics.PriceChangePercent < 0 ? -1 : 0;
            sy.LastPriceChangeDirection = st.LastPrice > sy.SymbolStatistics.LastPrice ? 1 : st.LastPrice < sy.SymbolStatistics.LastPrice ? -1 : 0;
            sy.SymbolStatistics.LastPrice = st.LastPrice.Trim(sy.PricePrecision);
            sy.SymbolStatistics.Volume = Convert.ToInt64(st.Volume);

            sy.SymbolStatistics.FirstTradeId = st.FirstTradeId;
            sy.SymbolStatistics.CloseTime = st.CloseTime;
            sy.SymbolStatistics.OpenTime = st.OpenTime;
            sy.SymbolStatistics.QuoteVolume = st.QuoteVolume;
            sy.SymbolStatistics.LowPrice = st.LowPrice;
            sy.SymbolStatistics.HighPrice = st.HighPrice;
            sy.SymbolStatistics.OpenPrice = st.OpenPrice;
            sy.SymbolStatistics.AskQuantity = st.AskQuantity;
            sy.SymbolStatistics.AskPrice = st.AskPrice;
            sy.SymbolStatistics.BidQuantity = st.BidQuantity;
            sy.SymbolStatistics.BidPrice = st.BidPrice;
            sy.SymbolStatistics.LastQuantity = st.LastQuantity;
            sy.SymbolStatistics.PreviousClosePrice = st.PreviousClosePrice;
            sy.SymbolStatistics.WeightedAveragePrice = st.WeightedAveragePrice;
            sy.SymbolStatistics.PriceChange = st.PriceChange;
            sy.SymbolStatistics.Period = st.Period;
            sy.SymbolStatistics.LastTradeId = st.LastTradeId;
            sy.SymbolStatistics.TradeCount = st.TradeCount;

            return sy;
        }

        public static Symbol JoinStatistics(this Symbol sy, SymbolStatistics st)
        {
            sy.SymbolStatistics = st;
            sy.PriceChangePercentDirection = sy.SymbolStatistics.PriceChangePercent > 0 ? 1 : sy.SymbolStatistics.PriceChangePercent < 0 ? -1 : 0;
            return sy;
        }
    }
}