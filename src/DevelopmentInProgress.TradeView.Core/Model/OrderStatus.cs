namespace DevelopmentInProgress.TradeView.Core.Model
{
    public enum OrderStatus
    {
        New,
        PartiallyFilled,
        Filled,
        Canceled,
        PendingCancel,
        Rejected,
        Expired
    }
}