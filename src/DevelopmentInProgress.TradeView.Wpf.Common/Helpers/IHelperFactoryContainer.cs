namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public interface IHelperFactoryContainer
    {
        IHelperFactory GetFactory<T>() where T : IHelperFactory;
    }
}
