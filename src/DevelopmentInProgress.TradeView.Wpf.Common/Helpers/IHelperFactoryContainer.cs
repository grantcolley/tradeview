namespace DevelopmentInProgress.TradeView.Wpf.Common.Helpers
{
    public interface IHelperFactoryContainer
    {
        T GetFactory<T>() where T : IHelperFactory;
    }
}
