using DevelopmentInProgress.MarketView.Interface.Interfaces;
using DevelopmentInProgress.MarketView.Interface.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.Events
{
    public class AggregateTradeEventArgs
    {
        public IEnumerable<ITrade> Trades { get; set; }
    }
}