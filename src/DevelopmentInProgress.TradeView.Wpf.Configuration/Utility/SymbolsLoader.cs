using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Configuration.View;
using DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Controls.Logging;
using Microsoft.Extensions.Logging;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.Utility
{
    public class SymbolsLoader : LoggingBase, ISymbolsLoader
    {
        private readonly IWpfExchangeService exchangeService;

        public SymbolsLoader(IWpfExchangeService exchangeService, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            this.exchangeService = exchangeService;
        }

        public void ShowSymbols(UserAccount userAccount)
        {
            using var viewModel = new SymbolsViewModel(exchangeService, userAccount, LoggerFactory);
            var view = new SymbolsView(viewModel);
            view.ShowDialog();
        }
    }
}
