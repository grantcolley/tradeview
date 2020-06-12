using DevelopmentInProgress.TradeView.Core.Enums;
using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Core.Model
{
    public class AccountInfo
    {
        public User User { get; set; }
        public Exchange Exchange { get; set; }
        public AccountCommissions Commissions { get; set; }
        public AccountStatus Status { get; set; }
        public DateTime Time { get; set; }
        public List<AccountBalance> Balances { get; set; }
    }
}
