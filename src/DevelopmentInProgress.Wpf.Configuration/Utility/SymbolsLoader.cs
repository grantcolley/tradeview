using DevelopmentInProgress.Wpf.Common.Model;
using DevelopmentInProgress.Wpf.Common.Services;
using DevelopmentInProgress.Wpf.Configuration.View;
using DevelopmentInProgress.Wpf.Configuration.ViewModel;
using Prism.Logging;

namespace DevelopmentInProgress.Wpf.Configuration.Utility
{
    public class SymbolsLoader : ISymbolsLoader
    {
        private IWpfExchangeService exchangeService;
        private ILoggerFacade logger;

        public SymbolsLoader(IWpfExchangeService exchangeService, ILoggerFacade logger)
        {
            this.exchangeService = exchangeService;
            this.logger = logger;
        }

        public void ShowSymbols(UserAccount userAccount)
        {
            using (var viewModel = new SymbolsViewModel(exchangeService, userAccount, logger))
            {
                var view = new SymbolsView(viewModel);
                view.ShowDialog();
            }
        }
    }
}
