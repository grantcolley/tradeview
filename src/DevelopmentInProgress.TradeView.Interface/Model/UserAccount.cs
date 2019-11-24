using DevelopmentInProgress.TradeView.Interface.Enums;

namespace DevelopmentInProgress.TradeView.Interface.Model
{
    public class UserAccount
    {
        public UserAccount()
        {
            Preferences = new Preferences();
        }

        public string AccountName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string ApiPassPhrase { get; set; }
        public Exchange Exchange { get; set; }
        public Preferences Preferences { get; set; }
    }
}
