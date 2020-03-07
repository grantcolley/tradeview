namespace DevelopmentInProgress.TradeView.Interface.Server
{
    public interface IServer
    {
        string Name { get; set; }
        string Url { get; set; }
        int MaxDegreeOfParallelism { get; set; }
    }
}
