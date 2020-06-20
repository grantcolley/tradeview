using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Core.Model
{
    public class UserAccounts
    {
        public UserAccounts()
        {
            Accounts = new List<UserAccount>();
        }

        public List<UserAccount> Accounts { get; }
    }
}
