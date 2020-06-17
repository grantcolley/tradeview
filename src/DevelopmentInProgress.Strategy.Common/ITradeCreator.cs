using DevelopmentInProgress.TradeView.Core.Interfaces;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;

namespace DevelopmentInProgress.Strategy.Common
{
    public interface ITradeCreator<T, TP>   where T : ITrade
                                            where TP : StrategyParameters
    {
        void Reset(TP parameters);
        T CreateTrade(ITrade trade);
    }
}