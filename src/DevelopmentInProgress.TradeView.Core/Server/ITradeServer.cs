namespace DevelopmentInProgress.TradeView.Core.Server
{
    public interface ITradeServer
    {
        string Name { get; set; }
        string Url { get; set; }
        int MaxDegreeOfParallelism { get; set; }
    }
}
