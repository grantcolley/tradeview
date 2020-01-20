﻿using DevelopmentInProgress.TradeView.Interface.Interfaces;
using System;

namespace DevelopmentInProgress.Strategy.Common
{
    public class TradeCache<T> where T : ITrade, new()
    {
        private T[] trades;

        public TradeCache(int incrementalSize)
        {
            IncrementalSize = incrementalSize;
            trades = new T[0];
        }

        public int CacheSize { get { return trades.Length; } }
        public int IncrementalSize { get; private set; }
        public int Position { get; private set; }

        public T[] GetTrades()
        {
            return trades;
        }

        public T GetLastTrade()
        {
            return trades[Position];
        }

        public T[] GetLastTrades(int maxLength)
        {
            if (maxLength > Position)
            {
                T[] lastTrades = new T[Position];
                Array.Copy(trades, lastTrades, Position);
                return lastTrades;
            }
            else
            {
                T[] lastTrades = new T[maxLength];
                Array.Copy(trades, Position - maxLength, lastTrades, 0, maxLength);
                return lastTrades;
            }
        }

        public T Add(ITrade trade)
        {
            Position += 1;

            var t = Create(trade);

            trades[Position] = t;

            if (Position == trades.Length)
            {
                Array.Resize(ref trades, IncrementalSize);
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
            return new T()
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
}