using DevelopmentInProgress.Strategy.Common.Parameter;
using DevelopmentInProgress.Strategy.Common.StrategyTrade;
using DevelopmentInProgress.TradeView.Interface.Interfaces;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DevelopmentInProgress.Strategy.Common.Test")]
namespace DevelopmentInProgress.Strategy.Common.TradeCreator
{
    public class MovingAverageTradeCreator : ITradeCreator<MovingAverageTrade, MovingAverageTradeParameters>
    {
        private decimal[] range;
        private int currentPeriod;
        private int movingAvarageRange;
        private decimal buyIndicator;
        private decimal sellIndicator;

        private object tradeLock = new object();

        public MovingAverageTradeCreator()
        {
            Reset(new MovingAverageTradeParameters());
        }

        public MovingAverageTrade CreateTrade(ITrade trade)
        {
            lock(tradeLock)
            { 
                return new MovingAverageTrade
                {
                    Symbol = trade.Symbol,
                    Exchange = trade.Exchange,
                    Id = trade.Id,
                    Price = trade.Price,
                    Quantity = trade.Quantity,
                    Time = trade.Time,
                    IsBuyerMaker = trade.IsBuyerMaker,
                    IsBestPriceMatch = trade.IsBestPriceMatch
                };
            }
        }

        public void Reset(MovingAverageTradeParameters parameters)
        {
            lock (tradeLock)
            {
                buyIndicator = parameters.BuyIndicator;
                sellIndicator = parameters.SellIndicator;
                var newMovingAvarageRange = parameters.MovingAvarageRange;

                if (range == null)
                {
                    currentPeriod = 0;
                    range = new decimal[newMovingAvarageRange];
                }
                else if (movingAvarageRange > newMovingAvarageRange)
                {
                    var newRange = new decimal[newMovingAvarageRange];

                    var sourceIndex = (movingAvarageRange - newMovingAvarageRange) - 1;
                    Array.Copy(range, sourceIndex, newRange, 0, newMovingAvarageRange);

                    currentPeriod = newMovingAvarageRange -1;
                    range = newRange;
                }
                else if (movingAvarageRange < newMovingAvarageRange)
                {
                    var newRange = new decimal[newMovingAvarageRange];

                    Array.Copy(range, newRange, movingAvarageRange);

                    currentPeriod = newMovingAvarageRange - 1;
                    range = newRange;
                }

                movingAvarageRange = newMovingAvarageRange;
            }
        }

        internal decimal[] GetRange()
        {
            var len = range.Length;
            var copy = new decimal[len];
            Array.Copy(range, copy, len);
            return copy;
        }

        internal int GetCurrentPeriod()
        {
            return currentPeriod;
        }

        internal int GetMovingAvarageRange()
        {
            return movingAvarageRange;
        }

        internal decimal GetBuyIndicator()
        {
            return buyIndicator;
        }

        internal decimal GetSellIndicator()
        {
            return sellIndicator;
        }
    }
}