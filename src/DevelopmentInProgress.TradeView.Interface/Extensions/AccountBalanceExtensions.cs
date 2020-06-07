using DevelopmentInProgress.TradeView.Interface.Model;
using System;

namespace DevelopmentInProgress.TradeView.Interface.Extensions
{
    public static class AccountBalanceExtensions
    {
        public static bool HasAvailableFunds(this AccountBalance ab, decimal price, decimal quantity)
        {
            if (ab == null)
            {
                throw new ArgumentNullException(nameof(ab));
            }

            return ab.Free >= price * quantity;
        }

        public static bool HasAvailableQuantity(this AccountBalance ab, decimal quantity)
        {
            if (ab == null)
            {
                throw new ArgumentNullException(nameof(ab));
            }

            return ab.Free >= quantity;
        }
    }
}