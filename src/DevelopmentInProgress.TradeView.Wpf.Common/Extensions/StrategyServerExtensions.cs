using DevelopmentInProgress.TradeView.Wpf.Common.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class StrategyServerExtensions
    {
        public static StrategyServer ToWpfStrategyServer(this Interface.Strategy.StrategyServer strategyServer)
        {
            return new StrategyServer
            {
                Name = strategyServer.Name,
                Url = strategyServer.Url
            };
        }

        public static Interface.Strategy.StrategyServer ToInterfaceStrategyServer(this StrategyServer strategyServer)
        {
            return new Interface.Strategy.StrategyServer
            {
                Name = strategyServer.Name,
                Url = strategyServer.Url
            };
        }
    }
}