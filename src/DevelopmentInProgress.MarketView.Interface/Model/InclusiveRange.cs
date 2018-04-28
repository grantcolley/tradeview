namespace DevelopmentInProgress.MarketView.Interface.Model
{
    public class InclusiveRange
    {
        private decimal increment;

        public decimal Minimum { get; set; }
        public decimal Maximum { get; set; }       

        public decimal Increment
        {
            get { return increment; }
            set
            {
                increment = value/1.00000000000000000000000000000m;
            }
        }
    }
}
