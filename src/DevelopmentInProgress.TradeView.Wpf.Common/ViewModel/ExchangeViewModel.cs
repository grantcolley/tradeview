using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using Microsoft.Extensions.Logging;

namespace DevelopmentInProgress.TradeView.Wpf.Common.ViewModel
{
    public abstract class ExchangeViewModel : BaseViewModel
    {
        public ExchangeViewModel(IWpfExchangeService exchangeService, ILogger logger)
            : base(logger)
        {
            ExchangeService = exchangeService;
        }

        protected IWpfExchangeService ExchangeService { get; private set; }
    }
}
