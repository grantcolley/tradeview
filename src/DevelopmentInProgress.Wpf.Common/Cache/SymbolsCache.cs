using System.Collections.Generic;
using DevelopmentInProgress.Wpf.Common.Model;

namespace DevelopmentInProgress.Wpf.Common.Cache
{
    public class SymbolsCache : ISymbolsCache
    {
        public SymbolsCache()
        {
            Symbols = new List<Symbol>();
        }

        public List<Symbol> Symbols { get; }
    }
}
