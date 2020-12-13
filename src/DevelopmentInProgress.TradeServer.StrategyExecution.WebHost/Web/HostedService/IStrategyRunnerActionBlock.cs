using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Web.HostedService
{
    public interface IStrategyRunnerActionBlock
    {
        ActionBlock<StrategyRunnerActionBlockInput> ActionBlock { get; set; }
        Task RunStrategyAsync(StrategyRunnerActionBlockInput strategyRunnerActionBlockInput);
    }
}
