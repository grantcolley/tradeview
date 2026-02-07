using DevelopmentInProgress.TradeView.Core.Model;

namespace DevelopmentInProgress.TradeView.Api.BinanceNet
{
    public static class OrderSideExtensions
    {
        public static Binance.Net.Enums.OrderSide ToBinanceOrderSide(this OrderSide order)
        {
            return order switch
            {
                OrderSide.Buy => Binance.Net.Enums.OrderSide.Buy,
                OrderSide.Sell => Binance.Net.Enums.OrderSide.Sell,
                _ => throw new NotImplementedException(),
            };
        }

        public static OrderSide ToTradeViewOrderSide(this Binance.Net.Enums.OrderSide order)
        {
            return order switch
            {
                Binance.Net.Enums.OrderSide.Buy => OrderSide.Buy,
                Binance.Net.Enums.OrderSide.Sell => OrderSide.Sell,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
