using DevelopmentInProgress.TradeView.Core.Model;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Core.Extensions
{
    public static class AccountInfoExtensions
    {
        public static AccountInfo Clone(this AccountInfo ai)
        {
            if (ai == null)
            {
                throw new ArgumentNullException(nameof(ai));
            }

            var accountInfo = new AccountInfo
            {
                User = new User 
                {
                    ApiKey = ai.User.ApiKey, 
                    ApiSecret = ai.User.ApiSecret, 
                    ApiPassPhrase = ai.User.ApiPassPhrase, 
                    AccountName = ai.User.AccountName, 
                    Exchange = ai.User.Exchange 
                },
                Exchange = ai.Exchange,
                Status = new AccountStatus { CanDeposit = ai.Status.CanDeposit, CanTrade = ai.Status.CanTrade, CanWithdraw = ai.Status.CanWithdraw },
                Commissions = new AccountCommissions { Buyer = ai.Commissions.Buyer, Maker = ai.Commissions.Maker, Seller = ai.Commissions.Seller, Taker = ai.Commissions.Taker },
                Time = ai.Time,
                Balances = new List<AccountBalance>()
            };

            foreach(var balance in ai.Balances)
            {
                accountInfo.Balances.Add(new AccountBalance { Asset = balance.Asset, Free = balance.Free, Locked = balance.Locked });
            }

            return accountInfo;
        }
    }
}
