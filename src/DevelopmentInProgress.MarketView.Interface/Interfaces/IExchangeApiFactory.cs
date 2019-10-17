using DevelopmentInProgress.MarketView.Interface.Enums;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.Interfaces
{
    public interface IExchangeApiFactory
    {
        IExchangeApi GetExchangeApi(Exchange exchange);
        Dictionary<Exchange, IExchangeApi> GetExchanges();        
    }
}
