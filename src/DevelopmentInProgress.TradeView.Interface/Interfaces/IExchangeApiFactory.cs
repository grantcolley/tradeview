using DevelopmentInProgress.TradeView.Interface.Enums;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Interface.Interfaces
{
    public interface IExchangeApiFactory
    {
        IExchangeApi GetExchangeApi(Exchange exchange);
        Dictionary<Exchange, IExchangeApi> GetExchanges();        
    }
}
