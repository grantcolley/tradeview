namespace DevelopmentInProgress.TradeView.Core.Server
{
    public interface IServer
    {
        string Name { get; set; }
        string Url { get; set; }
        int MaxDegreeOfParallelism { get; set; }
    }
}
