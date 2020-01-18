using DevelopmentInProgress.TradeView.Interface.Model;

namespace DevelopmentInProgress.Strategy.Demo
{
    public class DemoTrade : Trade
    {        
        public decimal SmaPrice { get; set; }
        public decimal BuyIndicatorPrice { get; set; }
        public decimal SellIndicatorPrice { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is DemoTrade))
            {
                return false;
            }
            else
            {
                return (Id == ((DemoTrade)obj).Id);
            }
        }

        public override int GetHashCode()
        {
            return (int)Id;
        }

        public override string ToString()
        {
            return $"{this.GetType().Name} {Id}";
        }
    }
}
