using DevelopmentInProgress.TradeView.Test.Helper;
using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Trading.Events;
using DevelopmentInProgress.TradeView.Wpf.Trading.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Prism.Logging;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

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
                if (args.Symbols.Any() // OnLoadedSymbols
                    || args.Value.Name.Equals("BNBBTC")) // OnSelectedSymbol
                {
                    // expected
                }
                else
                {
                    fail = true;
                }
            });

            var userAccount = new UserAccount
            {
                Preferences = new Preferences { SelectedSymbol = "BNBBTC", }
            };

            userAccount.Preferences.FavouriteSymbols.Add("BNBBTC");
            userAccount.Preferences.FavouriteSymbols.Add("TRXBTC");

            await symbolsViewModel.SetAccount(userAccount);

            await Task.Delay(1000);

            // Assert
            Assert.AreEqual(symbolsViewModel.Symbols.Count, 2);
            Assert.IsNotNull(symbolsViewModel.AccountPreferences);
            Assert.IsNotNull(symbolsViewModel.SelectedSymbol);
            Assert.AreEqual(symbolsViewModel.SelectedSymbol.Name, "BNBBTC");
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
                else if (args.Symbols.Any())
                {
                    // expected
                }
                else
                {
                    fail = true;
                }
            });

            var userAccount = new UserAccount
            {
                Preferences = new Preferences { SelectedSymbol = "BNBBTC", }
            };

            userAccount.Preferences.FavouriteSymbols.Add("BNBBTC");
            userAccount.Preferences.FavouriteSymbols.Add("TRXBTC");
            
            await symbolsViewModel.SetAccount(userAccount);

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
            await symbolsViewModel.SetAccount(account);

            await Task.Delay(1000);

            // Assert
            var favourites = from s in symbolsViewModel.Symbols
                             join f in account.Preferences.FavouriteSymbols on s.Name equals f
                             select s;

            foreach(var favourite in favourites)
            {
                Assert.IsNotNull(symbolsViewModel.Symbols.First(s => s.Name.Equals(favourite.Name)));
            }

            Assert.AreEqual(symbolsViewModel.SelectedSymbol.Name, account.Preferences.SelectedSymbol);
        }
    }
}
