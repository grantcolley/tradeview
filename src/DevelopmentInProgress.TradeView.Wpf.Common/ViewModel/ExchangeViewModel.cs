using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using Prism.Logging;

namespace DevelopmentInProgress.TradeView.Wpf.Common.ViewModel
{
    public abstract class ExchangeViewModel : BaseViewModel
    {
        public ExchangeViewModel(IWpfExchangeService exchangeService, ILoggerFacade logger)
            : base(logger)
        {
            ExchangeService = exchangeService;
        }

        protected IWpfExchangeService ExchangeService { get; private set; }
    }
}
