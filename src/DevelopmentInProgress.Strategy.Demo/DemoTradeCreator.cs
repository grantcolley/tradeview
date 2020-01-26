using DevelopmentInProgress.Strategy.Common;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Strategy.Demo
{
    public class DemoTradeCreator : ITradeCreator<DemoTrade, object>
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

        public void Reset(object parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}