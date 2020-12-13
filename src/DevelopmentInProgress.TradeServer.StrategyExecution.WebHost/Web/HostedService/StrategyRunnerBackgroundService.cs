using DevelopmentInProgress.TradeView.Core.Server;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DevelopmentInProgress.TradeServer.StrategyExecution.WebHost.Web.HostedService
{
    public class StrategyRunnerBackgroundService : BackgroundService
    {
        private readonly IServerMonitor serverMonitor;
        private readonly ILogger logger;
        private readonly IStrategyRunnerActionBlock strategyRunnerActionBlock;
        private CancellationToken cancellationToken;

        public StrategyRunnerBackgroundService(IServerMonitor serverMonitor, IStrategyRunnerActionBlock strategyRunnerActionBlock, ILoggerFactory loggerFactory)
        {
            this.serverMonitor = serverMonitor;
            this.strategyRunnerActionBlock = strategyRunnerActionBlock;

            logger = loggerFactory.CreateLogger<StrategyRunnerBackgroundService>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types")]
        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;

            logger.LogInformation("ExecuteAsync");

            try
            {
                strategyRunnerActionBlock.ActionBlock = new ActionBlock<StrategyRunnerActionBlockInput>(async actionBlockInput =>
                {
                    await actionBlockInput.StrategyRunner.RunAsync(actionBlockInput.Strategy, actionBlockInput.DownloadsPath, this.cancellationToken).ConfigureAwait(false);
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = serverMonitor.MaxDegreeOfParallelism });

                while (!this.cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, this.cancellationToken).ConfigureAwait(false);
                }

                strategyRunnerActionBlock.ActionBlock.Complete();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ExecuteAsync");
            }
        }
    }
}