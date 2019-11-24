using System.Collections.Generic;

namespace DevelopmentInProgress.TradeView.Interface.Model
{
    public class UserAccounts
    {
        public UserAccounts()
        {
            Accounts = new List<UserAccount>();
        }

        public List<UserAccount> Accounts { get; set; }
    }
}
