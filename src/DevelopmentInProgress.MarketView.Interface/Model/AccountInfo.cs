using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.MarketView.Interface.Model
{
    public class AccountInfo
    {
        public User User { get; set; }
        public AccountCommissions Commissions { get; set; }
        public AccountStatus Status { get; set; }
        public DateTime Time { get; set; }
        public List<AccountBalance> Balances { get; set; }
    }
}
