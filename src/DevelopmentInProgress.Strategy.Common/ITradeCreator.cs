using DevelopmentInProgress.TradeView.Interface.Interfaces;

namespace DevelopmentInProgress.Strategy.Common
{
    public interface ITradeCreator<T> where T : ITrade
    {
        T CreateTrade(ITrade trade);
    }
}
