using DevelopmentInProgress.TradeView.Wpf.Common.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class TradeServerExtensions
    {
        public static TradeServer ToWpfTradeServer(this Core.Server.TradeServer tradeServer)
        {
            return new TradeServer
            {
                Name = tradeServer.Name,
                Url = tradeServer.Url,
                MaxDegreeOfParallelism = tradeServer.MaxDegreeOfParallelism,
                Enabled = tradeServer.Enabled
            };
        }

        public static Core.Server.TradeServer ToCoreTradeServer(this TradeServer tradeServer)
        {
            return new Core.Server.TradeServer
            {
                Name = tradeServer.Name,
                Url = tradeServer.Url,
                MaxDegreeOfParallelism = tradeServer.MaxDegreeOfParallelism,
                Enabled = tradeServer.Enabled
            };
        }
    }
}