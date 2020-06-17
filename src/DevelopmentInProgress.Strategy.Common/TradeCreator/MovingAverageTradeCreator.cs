using DevelopmentInProgress.Strategy.Common.Parameter;
using DevelopmentInProgress.Strategy.Common.StrategyTrade;
using DevelopmentInProgress.TradeView.Core.Interfaces;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DevelopmentInProgress.Strategy.Common.Test")]
namespace DevelopmentInProgress.Strategy.Common.TradeCreator
{
    public class MovingAverageTradeCreator : ITradeCreator<MovingAverageTrade, MovingAverageTradeParameters>
    {
        private decimal[] range;
        private int position;
        private int movingAvarageRange;
        private decimal buyIndicator;
        private decimal sellIndicator;

        private object tradeLock = new object();

        public MovingAverageTradeCreator()
        {
            position = -1;
            Reset(new MovingAverageTradeParameters());
        }

        public MovingAverageTrade CreateTrade(ITrade trade)
        {
            if (trade == null)
            {
                throw new ArgumentNullException(nameof(trade));
            }

            lock (tradeLock)
            {
                position += 1;

                if (position == movingAvarageRange)
                {
                    // Reshuffle range i.e. remove oldest price and shuffle 
                    // the prices down. The new position is the last price
                    // in the range and will be replaced by the current trade price

                    position = movingAvarageRange - 1;

                    for (int i = 0; i < position; i++)
                    {
                        range[i] = range[i + 1];
                    }
                }

                range[position] = trade.Price;

                decimal sum = 0m;

                for (int i = 0; i <= position; i++)
                {
                    sum += range[i];
                }

                // The moving average is the sum of all the prices in the range divided 
                // by the number of prices in the range i.e. the position in the range
                var movingAverage = (position == 0) ? range[0] : sum / (position + 1);

                return new MovingAverageTrade
                {
                    Symbol = trade.Symbol,
                    Exchange = trade.Exchange,
                    Id = trade.Id,
                    Price = trade.Price,
                    Quantity = trade.Quantity,
                    Time = trade.Time,
                    IsBuyerMaker = trade.IsBuyerMaker,
                    IsBestPriceMatch = trade.IsBestPriceMatch,
                    MovingAveragePrice = movingAverage,
                    BuyPrice = movingAverage - (movingAverage * buyIndicator),
                    SellPrice = movingAverage + (movingAverage * sellIndicator)
                };
            }
        }

        public void Reset(MovingAverageTradeParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            lock (tradeLock)
            {
                buyIndicator = parameters.BuyIndicator;
                sellIndicator = parameters.SellIndicator;
                var newMovingAvarageRange = parameters.MovingAvarageRange;

                if (range == null
                    || position == -1)
                {
                    range = new decimal[newMovingAvarageRange];
                }
                else if (movingAvarageRange > newMovingAvarageRange)
                {
                    var newRange = new decimal[newMovingAvarageRange];

                    var sourceIndex = (movingAvarageRange - newMovingAvarageRange) - 1;
                    Array.Copy(range, sourceIndex, newRange, 0, newMovingAvarageRange);

                    position = newMovingAvarageRange -1;
                    range = newRange;
                }
                else if (movingAvarageRange < newMovingAvarageRange)
                {
                    var newRange = new decimal[newMovingAvarageRange];

                    Array.Copy(range, newRange, position + 1);

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

        internal int GetCurrentPosition()
        {
            return position;
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