using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Trading.Events
{
    public class AccountEventArgs : EventArgsBase<Account>
    {
        public AccountBalance SelectedAsset { get; set; }
        public AccountEventType AccountEventType { get; set; }
    }
}