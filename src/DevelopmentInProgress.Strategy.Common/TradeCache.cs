using DevelopmentInProgress.TradeView.Interface.Interfaces;
using System;
using System.Linq;

namespace DevelopmentInProgress.Strategy.Common
{
    public class TradeCache<T> where T : ITrade
    {
        private T[] trades;
        private ITradeCreator<T> tradeCreator;

        public TradeCache(ITradeCreator<T> tradeCreator, int incrementalSize)
        {
            IncrementalSize = incrementalSize;
            this.tradeCreator = tradeCreator;

            trades = new T[incrementalSize];
            Position = -1;
        }

        public int CacheSize { get { return trades.Length; } }
        public int IncrementalSize { get; private set; }
        public int Position { get; private set; }

        public T[] GetTrades()
        {
            if(Position.Equals(-1))
            {
                return default;
            }

            return trades.Take(Position + 1).ToArray();
        }

        public T GetLastTrade()
        {
            if(Position.Equals(-1))
            {
                return default;
            }

            return trades[Position];
        }

        public T[] GetLastTrades(int length)
        {
            if (Position.Equals(-1))
            {
                return default;
            }

            var pos = Position + 1;

            if (length > Position)
            {
                T[] lastTrades = new T[pos];
                Array.Copy(trades, lastTrades, pos);
                return lastTrades;
            }
            else
            {
                T[] lastTrades = new T[length];
                Array.Copy(trades, pos - length, lastTrades, 0, length);
                return lastTrades;
            }
        }

        public T Add(ITrade trade)
        {
            Position += 1;

            var t = Create(trade);

            trades[Position] = t;

            if (Position == trades.Length - 1)
            {
                Array.Resize(ref trades, trades.Length + IncrementalSize);
            }

            return t;
        }

        public T[] AddRange(ITrade[] newTrades)
        {
            int length = newTrades.Length;

            var range = new T[length];

            for (int i = 0; i < length; i++)
            {
                range[i] = Add(newTrades[i]);
            }

            return range;
        }

        private T Create(ITrade trade)
        {
            return tradeCreator.CreateTrade(trade);
        }
    }
}
