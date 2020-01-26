using DevelopmentInProgress.TradeView.Interface.Interfaces;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Strategy.Common.Test
{
    public class TestTradeCreator : ITradeCreator<TestTrade, object>
    {
        public TestTrade CreateTrade(ITrade trade)
        {
            return new TestTrade
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