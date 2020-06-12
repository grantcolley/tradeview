using DevelopmentInProgress.TradeView.Core.Enums;
using DevelopmentInProgress.TradeView.Core.Extensions;

namespace DevelopmentInProgress.TradeView.Wpf.Common.Model
{
    public class StrategySubscription : EntityBase
    {
        private string candlestickInterval;

        public string Symbol { get; set; }
        public int Limit { get; set; }
        public string AccountName { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string ApiPassPhrase { get; set; }
        public Exchange Exchange { get; set; }
        public bool SubscribeAccount { get; set; }
        public bool SubscribeOrderBook { get; set; }
        public bool SubscribeTrades { get; set; }
        public bool SubscribeCandlesticks { get; set; }

        public string SelectedExchange
        {
            get { return Exchange.ToString(); }
            set
            {
                if (value == null)
                {
                    Exchange = Exchange.Unknown;
                }
                else
                {
                    Exchange = ExchangeExtensions.GetExchange(value);
                }
            }
        }

        public string CandlestickInterval
        {
            get { return candlestickInterval; }
            set
            {
                if(candlestickInterval != value)
                {
                    candlestickInterval = value;
                    OnPropertyChanged("CandlestickInterval");
                }
            }
        }
    }
}