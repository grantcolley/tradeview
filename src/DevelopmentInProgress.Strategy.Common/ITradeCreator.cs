using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.Strategy;

namespace DevelopmentInProgress.Strategy.Common
{
    public interface ITradeCreator<T, P>    where T : ITrade
                                            where P : StrategyParameters
    {
        void Reset(P parameters);
        T CreateTrade(ITrade trade);
    }
}