namespace DevelopmentInProgress.TradeView.Wpf.Data
{
    public interface ITradeViewConfiguration
    {
        ITradeViewConfigurationAccounts Accounts { get; }
        ITradeViewConfigurationStrategy Strategy { get; }
    }
}