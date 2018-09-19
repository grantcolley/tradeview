using DevelopmentInProgress.Wpf.Common.Services;

namespace DevelopmentInProgress.Wpf.Common.ViewModel
{
    public abstract class ExchangeViewModel : BaseViewModel
    {
        public ExchangeViewModel(IWpfExchangeService exchangeService)
        {
            ExchangeService = exchangeService;
        }

        protected IWpfExchangeService ExchangeService { get; private set; }
    }
}
