using System.Collections.Generic;

namespace DevelopmentInProgress.Wpf.MarketView.Model
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
