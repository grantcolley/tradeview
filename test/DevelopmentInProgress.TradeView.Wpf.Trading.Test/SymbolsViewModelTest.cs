using DevelopmentInProgress.TradeView.Test.Helper;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Trading.Events;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Trading.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Prism.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DevelopmentInProgress.TradeView.Wpf.Trading.Test
{
    [TestClass]
    public class SymbolsViewModelTest
    {
        [TestMethod]
        public async Task SymbolsViewModel_SetAccount()
        {
            // Arrange
            var fail = false;
            var exchangeApi = ExchangeServiceHelper.GetExchangeService(ExchangeServiceType.SymbolsViewModel);
            var exchangeService = new WpfExchangeService(exchangeApi);
            var symbolsCacheFactory = SymbolsCacheFactoryHelper.GetSymbolsCachefactory(exchangeService);

            // Act
            var symbolsViewModel = new SymbolsViewModel(exchangeService, symbolsCacheFactory, new DebugLogger());

            var symbolsObservable = Observable.FromEventPattern<SymbolsEventArgs>(
                eventHandler => symbolsViewModel.OnSymbolsNotification += eventHandler,
                eventHandler => symbolsViewModel.OnSymbolsNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            symbolsObservable.Subscribe((args) =>
            {
                if (args.Symbols != null)
                {
                    // expected
                }
                else
                {
                    fail = true;
                }
            });

            symbolsViewModel.SetAccount(new UserAccount { Preferences = new Preferences { FavouriteSymbols = new ObservableCollection<string>(new[] { "BNBBTC" }) } });

            await Task.Delay(1000);

            // Assert
            Assert.AreEqual(symbolsViewModel.Symbols.Count, TestHelper.Symbols.Count);
            Assert.IsNotNull(symbolsViewModel.AccountPreferences);
            Assert.IsNull(symbolsViewModel.SelectedSymbol);
            Assert.IsFalse(fail);
        }

        [TestMethod]
        public async Task SelectedSymbol()
        {
            // Arrange
            var fail = false;
            Symbol selectedSymbol = null;
            var exchangeApi = ExchangeServiceHelper.GetExchangeService(ExchangeServiceType.SymbolsViewModel);
            var exchangeService = new WpfExchangeService(exchangeApi);
            var symbolsCacheFactory = SymbolsCacheFactoryHelper.GetSymbolsCachefactory(exchangeService);
            var symbolsViewModel = new SymbolsViewModel(exchangeService, symbolsCacheFactory, new DebugLogger());

            var symbolsObservable = Observable.FromEventPattern<SymbolsEventArgs>(
                eventHandler => symbolsViewModel.OnSymbolsNotification += eventHandler,
                eventHandler => symbolsViewModel.OnSymbolsNotification -= eventHandler)
                .Select(eventPattern => eventPattern.EventArgs);

            symbolsObservable.Subscribe((args) =>
            {
                if (args.Value != null)
                {
                    selectedSymbol = args.Value;
                }
                else if (args.Symbols != null)
                {
                    // expected
                }
                else
                {
                    fail = true;
                }
            });

            symbolsViewModel.SetAccount(new UserAccount { Preferences = new Preferences { FavouriteSymbols = new ObservableCollection<string>(new[] { "BNBBTC" }) } });

            await Task.Delay(1000);

            // Act
            var trx = symbolsViewModel.Symbols.Single(s => s.Name.Equals("TRXBTC"));
            symbolsViewModel.SelectedSymbol = trx;

            // Assert
            Assert.AreEqual(symbolsViewModel.SelectedSymbol, selectedSymbol);
            Assert.IsFalse(fail);
        }

        [TestMethod]
        public async Task AccountPreferences()
        {
            // Arrange
            var exchangeApi = ExchangeServiceHelper.GetExchangeService(ExchangeServiceType.SymbolsViewModel);
            var exchangeService = new WpfExchangeService(exchangeApi);
            var symbolsCacheFactory = SymbolsCacheFactoryHelper.GetSymbolsCachefactory(exchangeService);
            var symbolsViewModel = new SymbolsViewModel(exchangeService, symbolsCacheFactory, new DebugLogger());
            
            var userData = File.ReadAllText("UserAccounts.txt");
            var accounts = JsonConvert.DeserializeObject<UserAccounts>(userData);

            var account = accounts.Accounts.First();

            // Act
            symbolsViewModel.SetAccount(account);

            await Task.Delay(1000);

            // Assert
            var favourites = from s in symbolsViewModel.Symbols
                             join f in account.Preferences.FavouriteSymbols on s.Name equals f
                             select s;

            foreach(var favourite in favourites)
            {
                Assert.IsNotNull(symbolsViewModel.Symbols.First(s => s.Name.Equals(favourite.Name) && s.IsFavourite.Equals(true)));
            }

            Assert.AreEqual(symbolsViewModel.SelectedSymbol.Name, account.Preferences.SelectedSymbol);
        }
    }
}
