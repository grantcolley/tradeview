namespace DevelopmentInProgress.Wpf.Common.Model
{
    public class UserAccount : EntityBase
    {
        public string AccountName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public Preferences Preferences { get; set; }
    }
}
