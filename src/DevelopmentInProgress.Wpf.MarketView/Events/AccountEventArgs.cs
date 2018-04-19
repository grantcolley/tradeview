using DevelopmentInProgress.Wpf.MarketView.Model;

namespace DevelopmentInProgress.Wpf.MarketView.Events
{
    public class AccountEventArgs : EventArgsBase<Account>
    {
        public AccountBalance SelectedAsset { get; set; }
        public bool UpdateOrders { get; set; }
    }
}