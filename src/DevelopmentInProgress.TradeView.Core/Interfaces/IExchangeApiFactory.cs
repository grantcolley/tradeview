using DevelopmentInProgress.TradeView.Core.Enums;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Core.Interfaces
{
    public interface IExchangeApiFactory
    {
        IExchangeApi GetExchangeApi(Exchange exchange);
        Dictionary<Exchange, IExchangeApi> GetExchanges();        
    }
}
