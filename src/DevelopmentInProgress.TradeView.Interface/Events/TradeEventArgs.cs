using DevelopmentInProgress.TradeView.Interface.Interfaces;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Interface.Events
{
    public class TradeEventArgs
    {
        public IEnumerable<ITrade> Trades { get; set; }
    }
}