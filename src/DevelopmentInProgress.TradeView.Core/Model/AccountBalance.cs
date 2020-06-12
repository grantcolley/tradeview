namespace DevelopmentInProgress.TradeView.Core.Model
{
    public class AccountBalance
    {
        public string Asset { get; set; }
        public decimal Free { get; set; }
        public decimal Locked { get; set; }
    }
}