using DevelopmentInProgress.Wpf.Common.Services;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public abstract class BaseViewModel : Common.ViewModel.BaseViewModel
    {
        public BaseViewModel(IWpfExchangeService exchangeService)
        {
            ExchangeService = exchangeService;
        }

        protected IWpfExchangeService ExchangeService { get; private set; }
    }
}
