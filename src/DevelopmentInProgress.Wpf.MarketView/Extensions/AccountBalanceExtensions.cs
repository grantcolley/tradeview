using DevelopmentInProgress.Wpf.MarketView.Model;
using Interface = DevelopmentInProgress.MarketView.Interface.Model;

namespace DevelopmentInProgress.Wpf.MarketView.Extensions
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