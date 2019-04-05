using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.MarketView.Events;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.Common.Services;
using DevelopmentInProgress.Wpf.MarketView.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Prism.Logging;

namespace DevelopmentInProgress.Wpf.MarketView.Test
{
    [TestClass]
    public class SymbolsViewModelTest
    {
        [TestMethod]
        public async Task SymbolsViewModel_Initialise()
        {
            // Arrange
            var fail = false;
            var exchangeApi = ExchangeServiceHelper.GetExchangeService(ExchangeServiceType.SymbolsViewModel);
            var exchangeService = new WpfExchangeService(exchangeApi);
            var symbolsCache = SymbolsCacheHelper.GetSymbolsCache(exchangeService);

            // Act
            var symbolsViewModel = new SymbolsViewModel(exchangeService, symbolsCache, new DebugLogger());

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

            await Task.Delay(1000);

            // Assert
            Assert.AreEqual(symbolsViewModel.Symbols.Count, TestHelper.Symbols.Count);
            Assert.IsNull(symbolsViewModel.AccountPreferences);
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
            var symbolsCache = SymbolsCacheHelper.GetSymbolsCache(exchangeService);
            var symbolsViewModel = new SymbolsViewModel(exchangeService, symbolsCache, new DebugLogger());

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
            var symbolsCache = SymbolsCacheHelper.GetSymbolsCache(exchangeService);
            var symbolsViewModel = new SymbolsViewModel(exchangeService, symbolsCache, new DebugLogger());
            
            var userData = File.ReadAllText("UserAccounts.txt");
            var accounts = JsonConvert.DeserializeObject<UserAccounts>(userData);

            var account = accounts.Accounts.First();

            await Task.Delay(1000);

            // Act
            symbolsViewModel.SetAccount(account);

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
