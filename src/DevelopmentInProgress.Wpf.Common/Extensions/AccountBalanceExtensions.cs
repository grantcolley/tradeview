using DevelopmentInProgress.Wpf.Common.Model;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.Common.Extensions
{
    public static class AccountBalanceExtensions
    {
        public static Interface.AccountBalance GetInterfaceAccountBalance(this AccountBalance ab)
        {
            return new Interface.AccountBalance
            {
                Asset = ab.Asset,
                Free = ab.Free,
                Locked = ab.Locked
            };
        }
    }
}