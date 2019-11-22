using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Wpf.Trading.Events
{
    public class SymbolsEventArgs : EventArgsBase<Symbol>
    {
        public List<Symbol> Symbols { get; set; }
    }
}