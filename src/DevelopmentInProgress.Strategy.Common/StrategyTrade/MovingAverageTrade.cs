using DevelopmentInProgress.TradeView.Core.Model;

namespace DevelopmentInProgress.Strategy.Common.StrategyTrade
{
    public class MovingAverageTrade : Trade
    {
        public decimal MovingAveragePrice { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is MovingAverageTrade))
            {
                return false;
            }
            else
            {
                return (Id == ((MovingAverageTrade)obj).Id);
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
