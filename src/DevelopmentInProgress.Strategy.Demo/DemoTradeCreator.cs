using DevelopmentInProgress.Strategy.Common;
using DevelopmentInProgress.TradeView.Interface.Interfaces;

namespace DevelopmentInProgress.Strategy.Demo
{
    public class DemoTradeCreator : ITradeCreator<DemoTrade>
    {
        public DemoTrade CreateTrade(ITrade trade)
        {
            return new DemoTrade
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