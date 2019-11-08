using DevelopmentInProgress.Wpf.Common.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Wpf.Common.Cache
{
    public interface ISymbolsCache
    {
        event EventHandler<Exception> OnSymbolsCacheException;
        Task<List<Symbol>> GetSymbols(IEnumerable<string> subscriptions);
        void ValueAccount(Account account);
    }
}