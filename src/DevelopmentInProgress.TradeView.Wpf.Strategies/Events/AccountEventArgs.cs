using DevelopmentInProgress.TradeView.Wpf.Common.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Strategies.Events
{
    public class AccountEventArgs : EventArgsBase<Account>
    {
        public AccountEventType AccountEventType { get; set; }
    }
}
