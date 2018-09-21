using DevelopmentInProgress.Wpf.Common.Events;
using DevelopmentInProgress.Wpf.Common.Model;

namespace DevelopmentInProgress.Wpf.StrategyManager.Events
{
    public class AccountEventArgs : EventArgsBase<Account>
    {
        public AccountEventType AccountEventType { get; set; }
    }
}
