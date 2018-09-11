using DevelopmentInProgress.Wpf.Common.Model;
using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.Common.Cache
{
    public interface ISymbolsCache
    {
        List<Symbol> Symbols { get; }
    }
}