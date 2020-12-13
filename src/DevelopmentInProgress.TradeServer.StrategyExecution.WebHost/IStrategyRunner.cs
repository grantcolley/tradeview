using System.Threading;
using System.Threading.Tasks;
using DevelopmentInProgress.TradeView.Core.TradeStrategy;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost
{
    public interface IStrategyRunner
    {
        Task<Strategy> RunAsync(Strategy strategy, string localPath, CancellationToken cancellationToken);
    }
}
