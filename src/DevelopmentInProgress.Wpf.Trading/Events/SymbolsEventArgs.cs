using DevelopmentInProgress.Wpf.Common.Events;
using DevelopmentInProgress.Wpf.Common.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.Trading.Events
{
    public class SymbolsEventArgs : EventArgsBase<Symbol>
    {
        public List<Symbol> Symbols { get; set; }
    }
}