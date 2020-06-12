using DevelopmentInProgress.TradeView.Core.Enums;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class UserAccount : EntityBase
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
