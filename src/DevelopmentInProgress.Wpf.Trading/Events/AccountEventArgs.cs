using DevelopmentInProgress.Wpf.Common.Events;
using DevelopmentInProgress.Wpf.Common.Model;

namespace DevelopmentInProgress.Wpf.Trading.Events
{
    public class AccountEventArgs : EventArgsBase<Account>
    {
        public AccountBalance SelectedAsset { get; set; }
        public AccountEventType AccountEventType { get; set; }
    }
}