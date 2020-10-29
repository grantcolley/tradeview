using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Cache
{
    public interface ISymbolsCache
    {
        event EventHandler<Exception> OnSymbolsCacheException;
        Task<List<Symbol>> GetSymbols(IEnumerable<string> subscriptions);
        void SubscribeAccountsAssets(IEnumerable<UserAccount> userAccounts);
        void ValueAccount(Account account);
    }
}