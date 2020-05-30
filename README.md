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
TradeView consists of modules, accessible from the navigation panel on the left, including [Configuration](#configuration), [Trading](#trading), [Strategies](#strategies) and [Dashboard](#dashboard).
  
![Alt text](/README-images/navigationpanel.PNG?raw=true "Navigation Panel")

#### Configuration
The Configuration module is where configuration for trading accounts, running strategies and strategy servers are managed.

* **Manage Accounts** allows you to create and persist trading accounts including account name, exchange, api key and secret key. It also persists display preferences for the trading screen such as favourite symbols, and default selected symbol.
* **Manage Strategies** persists strategy configuration
* **Manage Servers** persists trade server details for servers running strategies 

![Alt text](/README-images/configuration.PNG?raw=true "Configuration")

#### Trading
Selecting the Trading module in the navigation panel will list the trading accounts in the main panel. Clicking an account will open a trading document for that account in the main window.

A trading document consists of several panels including favourite symbols list, selected symbol panel showing live order book and trade feeds, account assets with real-time updates, a control for placing trades and an open order monitoring panel.

![Alt text](/README-images/tradeview.PNG?raw=true "Trade View")

#### Strategies
![Alt text](/README-images/strategies.PNG?raw=true "Strategies")

#### Dashboard
![Alt text](/README-images/dashboard.PNG?raw=true "Dashboard")
