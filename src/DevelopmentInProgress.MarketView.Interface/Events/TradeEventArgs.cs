using DevelopmentInProgress.MarketView.Interface.Interfaces;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.Events
{
    public class TradeEventArgs
    {
        public IEnumerable<ITrade> Trades { get; set; }
    }
}