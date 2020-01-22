using DevelopmentInProgress.Strategy.Common.StrategyTrade;
using DevelopmentInProgress.TradeView.Interface.Interfaces;

namespace DevelopmentInProgress.Strategy.Common.TradeCreator
{
    public class SmaTradeCreator : ITradeCreator<SmaTrade>
    {
        public SmaTrade CreateTrade(ITrade trade)
        {
            return new SmaTrade
            {
                Symbol = trade.Symbol,
                Exchange = trade.Exchange,
                Id = trade.Id,
                Price = trade.Price,
                Quantity = trade.Quantity,
                Time = trade.Time,
                IsBuyerMaker = trade.IsBuyerMaker,
                IsBestPriceMatch = trade.IsBestPriceMatch
            };
        }
    }
}