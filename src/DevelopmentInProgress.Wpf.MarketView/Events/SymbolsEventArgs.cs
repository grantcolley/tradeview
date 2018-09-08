using DevelopmentInProgress.Wpf.Common.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.MarketView.Events
{
    public class SymbolsEventArgs : EventArgsBase<Symbol>
    {
        public List<Symbol> Symbols { get; set; }
    }
}