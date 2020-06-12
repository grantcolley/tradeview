using DevelopmentInProgress.TradeView.Wpf.Common.Model;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class AccountBalanceExtensions
    {
        public static Core.Model.AccountBalance GetInterfaceAccountBalance(this AccountBalance ab)
        {
            return new Core.Model.AccountBalance
            {
                Asset = ab.Asset,
                Free = ab.Free,
                Locked = ab.Locked
            };
        }
    }
}