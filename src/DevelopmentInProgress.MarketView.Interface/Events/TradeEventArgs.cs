using DevelopmentInProgress.MarketView.Interface.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.Events
{
    public class TradeEventArgs
    {
        public IEnumerable<Trade> Trades { get; set; }
    }
}