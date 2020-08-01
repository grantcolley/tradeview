using DevelopmentInProgress.TradeView.Wpf.Common.Model;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Configuration.View;
using DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel;
using Prism.Logging;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.Utility
{
    public class SymbolsLoader : ISymbolsLoader
    {
        private readonly IWpfExchangeService exchangeService;
        private readonly ILoggerFacade logger;

        public SymbolsLoader(IWpfExchangeService exchangeService, ILoggerFacade logger)
        {
            this.exchangeService = exchangeService;
            this.logger = logger;
        }

        public void ShowSymbols(UserAccount userAccount)
        {
            using var viewModel = new SymbolsViewModel(exchangeService, userAccount, logger);
            var view = new SymbolsView(viewModel);
            view.ShowDialog();
        }
    }
}
