namespace DevelopmentInProgress.TradeView.Data
{
    public interface ITradeViewConfiguration
    {
        ITradeViewConfigurationAccounts Accounts { get; }
        ITradeViewConfigurationStrategy Strategy { get; }
    }
}