using DevelopmentInProgress.TradeView.Wpf.Common.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Events
{
    public class AccountEventArgs : BaseEventArgs<Account>
    {
        public AccountBalance SelectedAsset { get; set; }
        public AccountEventType AccountEventType { get; set; }
    }
}