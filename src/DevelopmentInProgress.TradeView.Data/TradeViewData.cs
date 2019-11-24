namespace DevelopmentInProgress.TradeView.Data
{
    public class TradeViewData : ITradeViewConfiguration
    {
        private ITradeViewData tradeViewData;

        public TradeViewData(ITradeViewData tradeViewData)
        {
            this.tradeViewData = tradeViewData;
        }

        public ITradeViewConfigurationAccounts Accounts { get { return tradeViewData.ConfigurationData.Accounts; } }

        public ITradeViewConfigurationStrategy Strategy { get { return tradeViewData.ConfigurationData.Strategy; } }
    }
}