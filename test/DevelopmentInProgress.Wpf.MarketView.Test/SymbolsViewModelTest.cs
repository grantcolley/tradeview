using DevelopmentInProgress.MarketView.Test.Helper;
using DevelopmentInProgress.Wpf.MarketView.Events;
using DevelopmentInProgress.Wpf.MarketView.Model;
using DevelopmentInProgress.Wpf.MarketView.Services;
using DevelopmentInProgress.Wpf.MarketView.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

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
            var exchangeApi = ExchangeApiHelper.GetExchangeApi(ExchangeApiType.SymbolsViewModel);
            var exchangeService = new ExchangeService(exchangeApi);

            // Act
            var symbolsViewModel = new SymbolsViewModel(exchangeService);

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
            Assert.AreEqual(symbolsViewModel.Symbols.Count, MarketHelper.Symbols.Count);
            Assert.IsNull(symbolsViewModel.User);
            Assert.IsNull(symbolsViewModel.SelectedSymbol);
            Assert.IsFalse(fail);
        }

        [TestMethod]
        public async Task SelectedSymbol()
        {
            // Arrange
            var fail = false;
            Symbol selectedSymbol = null;
            var exchangeApi = ExchangeApiHelper.GetExchangeApi(ExchangeApiType.SymbolsViewModel);
            var exchangeService = new ExchangeService(exchangeApi);
            var symbolsViewModel = new SymbolsViewModel(exchangeService);

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
        public void User()
        {
            Assert.Fail();
        }
    }
}
