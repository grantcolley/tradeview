namespace DevelopmentInProgress.TradeView.Interface.Server
{
    public class Server : IServer
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int MaxDegreeOfParallelism { get; set; }
        public bool Enabled { get; set; }
    }
}
