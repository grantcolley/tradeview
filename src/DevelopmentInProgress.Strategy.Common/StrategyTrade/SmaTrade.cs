using DevelopmentInProgress.TradeView.Interface.Model;

namespace DevelopmentInProgress.Strategy.Common.StrategyTrade
{
    public class SmaTrade : Trade
    {
        public decimal SmaPrice { get; set; }
        public decimal BuyIndicatorPrice { get; set; }
        public decimal SellIndicatorPrice { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SmaTrade))
            {
                return false;
            }
            else
            {
                return (Id == ((SmaTrade)obj).Id);
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
