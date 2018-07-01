using Binance;

namespace DevelopmentInProgress.MarketView.Api.Binance
{
    public static class OrderHelper
    {
        public static ClientOrder GetOrder(Interface.Model.User user, Interface.Model.ClientOrder clientOrder)
        {
            var apiUser = new BinanceApiUser(user.ApiKey, user.ApiSecret);
            var orderType = (OrderType)clientOrder.Type;
            switch (orderType)
            {
                case OrderType.Market:
                    return new MarketOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity };
                case OrderType.StopLoss:
                    return new StopLossOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, StopPrice = clientOrder.StopPrice };
                case OrderType.TakeProfit:
                    return new TakeProfitOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, StopPrice = clientOrder.StopPrice };
                case OrderType.Limit:
                    return new LimitOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, Price = clientOrder.Price };
                case OrderType.LimitMaker:
                    return new LimitMakerOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, Price = clientOrder.Price };
                case OrderType.StopLossLimit:
                    return new StopLossLimitOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, StopPrice = clientOrder.StopPrice, Price = clientOrder.Price };
                case OrderType.TakeProfitLimit:
                    return new TakeProfitLimitOrder(apiUser) { Symbol = clientOrder.Symbol, Side = (OrderSide)clientOrder.Side, Quantity = clientOrder.Quantity, StopPrice = clientOrder.StopPrice, Price = clientOrder.Price };
                default:
                    throw new System.Exception("Unknown order type.");
            }
        }
    }
}
