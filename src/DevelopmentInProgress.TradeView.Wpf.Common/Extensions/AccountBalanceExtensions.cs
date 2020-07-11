using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using System;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Extensions
{
    public static class AccountBalanceExtensions
    {
        public static Core.Model.AccountBalance GetCoreAccountBalance(this AccountBalance ab)
        {
            if(ab == null)
            {
                throw new ArgumentNullException(nameof(ab));
            }

            return new Core.Model.AccountBalance
            {
                Asset = ab.Asset,
                Free = ab.Free,
                Locked = ab.Locked
            };
        }
    }
}