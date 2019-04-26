using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.Strategy
{
    public class TradeNotification
    {
        public IEnumerable<ITrade> Trades { get; set; }
        public IEnumerable<MovingAverage> MovingAverages { get; set; }
    }
}
