namespace DevelopmentInProgress.TradeView.Interface.Model
{
    public enum SymbolStatus
    {
        PreTrading = 0,
        Trading = 1,
        PostTrading = 2,
        EndOfDay = 3,
        Halt = 4,
        AuctionMatch = 5,
        Break = 6
    }
}
