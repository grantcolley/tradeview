using System;

namespace DevelopmentInProgress.TradeView.Core.Server
{
    public interface ITradeServer
    {
        string Name { get; set; }
        Uri Uri { get; set; }
        int MaxDegreeOfParallelism { get; set; }
    }
}
