using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class TradeServerExtensions
    {
        public static TradeServer ToWpfTradeServer(this Core.Server.TradeServer tradeServer)
        {
            if (tradeServer == null)
            {
                throw new ArgumentNullException(nameof(tradeServer));
            }

            return new TradeServer
            {
                Name = tradeServer.Name,
                Uri = tradeServer.Uri,
                MaxDegreeOfParallelism = tradeServer.MaxDegreeOfParallelism,
                Enabled = tradeServer.Enabled
            };
        }

        public static Core.Server.TradeServer ToCoreTradeServer(this TradeServer tradeServer)
        {
            if (tradeServer == null)
            {
                throw new ArgumentNullException(nameof(tradeServer));
            }

            return new Core.Server.TradeServer
            {
                Name = tradeServer.Name,
                Uri = tradeServer.Uri,
                MaxDegreeOfParallelism = tradeServer.MaxDegreeOfParallelism,
                Enabled = tradeServer.Enabled
            };
        }
    }
}