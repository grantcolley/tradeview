using DevelopmentInProgress.TradeView.Interface.Interfaces;
using System.Threading.Tasks;

namespace DevelopmentInProgress.Strategy.Common
{
    public interface ITradeCreator<T, P> where T : ITrade
    {
        void Reset(P parameters);
        T CreateTrade(ITrade trade);
    }
}