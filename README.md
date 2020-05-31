# tradeview

[![Build status](https://ci.appveyor.com/api/projects/status/lsf9kuf5p93wvr0p/branch/master?svg=true)](https://ci.appveyor.com/project/grantcolley/tradeview/branch/master)

Alt currency trading application built on the [Origin](https://github.com/grantcolley/origin) framework.

#### Table of Contents
* [Overview](#overview)
  * [Configuration](#configuration)
  * [Trading](#trading)    
  * [Strategies](#strategies)
  * [Dashboard](#dashboard)

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
The Trading module shows a list of trading accounts in the navigation panel. Clicking an account will open a trading document in the main window for that account. From the trading document you can:
* see the account's balances
* view realtime pricing of favourite symbols
* select a symbol to subscribe to live orderbook and trade feed
* place buy and sell orders
* monitor and manage open orders in realtime 

![Alt text](/README-images/tradeview.PNG?raw=true "Trade View")

#### Strategies
Strategies are run on the [tradeserver](https://github.com/grantcolley/tradeserver) and can be monitored by one or more users. It is possible to update a running strategies parameters in realtime e.g. buy and sell triggers or suspend trading.

![Alt text](/README-images/strategies.PNG?raw=true "Strategies")

#### Dashboard
The dashboard shows all configured [tradeservers](https://github.com/grantcolley/tradeserver) and whether they are active. Active tradesers will show strategies currently running on them including a strategies parameters and its active connections i.e. users monitoring the strategy.

![Alt text](/README-images/dashboard.PNG?raw=true "Dashboard")

