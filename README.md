# tradeview

[![Build status](https://ci.appveyor.com/api/projects/status/lsf9kuf5p93wvr0p/branch/master?svg=true)](https://ci.appveyor.com/project/grantcolley/tradeview/branch/master)

Alt currency trading application built on the [Origin](https://github.com/grantcolley/origin) framework.

#### Table of Contents
* [Overview](#overview)
  * [Configuration](#configuration)
  * [Trading](#trading)    
  * [Strategies](#strategies)
  * [Dashboard](#dashboard)
* [Running a Strategy](#running-a-strategy)
* [Extending tradeview](#extending-tradeview)
  * [Adding a new Exchange API](#adding-a-new-exchange-api)
  * [Persisting Configuration Data](#persisting-configuration-data)

## Overview
**tradeview** consists of modules, accessible from the navigation panel on the left, including [Configuration](#configuration), [Trading](#trading), [Strategies](#strategies) and the [Dashboard](#dashboard).
  
![Alt text](/README-images/navigationpanel.PNG?raw=true "Navigation Panel")

#### Configuration
The Configuration module is where configuration for trading accounts, running strategies and strategy servers is managed.

* **Manage Accounts** allows you to create and persist trading accounts including account name, exchange, api key and secret key. It also persists display preferences for the trading screen, such as favourite symbols, and default selected symbol.
* **Manage Strategies** persists configuration for running a trading strategy and displaying a running strategy in realtime.
* **Manage Servers** persists trade server details for servers that run trading strategies 

![Alt text](/README-images/configuration.PNG?raw=true "Configuration")

#### Trading
The Trading module shows a list of trading accounts in the navigation panel. Selecting an account will open a trading document in the main window for that account. From the trading document you can:
* see the account's balances
* view realtime pricing of favourite symbols
* select a symbol to subscribe to live orderbook and trade feed
* place buy and sell orders
* monitor and manage open orders in realtime 

![Alt text](/README-images/tradeview.PNG?raw=true "Trade View")

#### Strategies
Strategies are run on an instance of [tradeserver](https://github.com/grantcolley/tradeserver) and can be monitored by one or more users. It is possible to update a running strategy's parameters in realtime e.g. buy and sell triggers or suspend trading. See [Running a Strategy](#running-a-strategy).

![Alt text](/README-images/strategies.PNG?raw=true "Strategies")

#### Dashboard
The dashboard shows all configured [tradeservers](https://github.com/grantcolley/tradeserver) and whether they are active. An active [tradeserver](https://github.com/grantcolley/tradeserver) will show the strategies currently running on it, including each strategy's parameters and its active connections i.e. which users are monitoring the strategy. See [Running a Strategy](#running-a-strategy).

![Alt text](/README-images/dashboard.PNG?raw=true "Dashboard")

## Running a Strategy
Strategies are run on an instance of [tradeserver](https://github.com/grantcolley/tradeserver), currently a private repository.

## Extending tradeview

#### Adding a new Exchange API
**tradeview** is intended to trade against multiple exchanges and the following api's are currently supported 
* [Binance](https://github.com/sonvister/Binance)
* [Kucoin.Net](https://github.com/JKorf/Kucoin.Net)

To add a new api create a new .NET Standard project for the API wrapper and create a class that implements [IExchangeApi](https://github.com/grantcolley/tradeview/blob/master/src/DevelopmentInProgress.TradeView.Interface/Interfaces/IExchangeApi.cs).
For example see [BinanceExchangeApi](https://github.com/grantcolley/tradeview/blob/master/src/DevelopmentInProgress.TradeView.Api.Binance/BinanceExchangeApi.cs). 

```C#
namespace DevelopmentInProgress.TradeView.Api.Binance
{
    public class BinanceExchangeApi : IExchangeApi
    {
        private IBinanceApi binanceApi;

        public BinanceExchangeApi()
        {
            binanceApi = new BinanceApi();
        }

        public async Task<Order> PlaceOrder(User user, ClientOrder clientOrder)
        {
            var order = OrderHelper.GetOrder(user, clientOrder);
            var result = await binanceApi.PlaceAsync(order).ConfigureAwait(false);
            return NewOrder(user, result);
        }

        public async Task<string> CancelOrderAsync(User user, string symbol, string orderId, string newClientOrderId = null, long recWindow = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var id = Convert.ToInt64(orderId);
            using (var apiUser = new BinanceApiUser(user.ApiKey, user.ApiSecret))
            {
                var result = await binanceApi.CancelOrderAsync(apiUser, symbol, id, newClientOrderId, recWindow, cancellationToken).ConfigureAwait(false);
                return result;
            }
        }
```

Next, add the exchange to the [Exchange](https://github.com/grantcolley/tradeview/blob/master/src/DevelopmentInProgress.TradeView.Interface/Enums/Exchange.cs) enum.

```C#
    public enum Exchange
    {
        Unknown,
        Binance,
        Kucoin,
        Test
    }
```

Finally, return an instance of the new exchange from the [ExchangeApiFactory](https://github.com/grantcolley/tradeview/blob/master/src/DevelopmentInProgress.TradeView.Service/ExchangeApiFactory.cs).

```C#
    public class ExchangeApiFactory : IExchangeApiFactory
    {
        public IExchangeApi GetExchangeApi(Exchange exchange)
        {
            switch(exchange)
            {
                case Exchange.Binance:
                    return new BinanceExchangeApi();
                case Exchange.Kucoin:
                    return new KucoinExchangeApi();
                default:
                    throw new NotImplementedException();
            }
        }

        public Dictionary<Exchange, IExchangeApi> GetExchanges()
        {
            var exchanges = new Dictionary<Exchange, IExchangeApi>();
            exchanges.Add(Exchange.Binance, GetExchangeApi(Exchange.Binance));
            exchanges.Add(Exchange.Kucoin, GetExchangeApi(Exchange.Kucoin));
            return exchanges;
        }
    }
```

#### Persisting Configuration Data
