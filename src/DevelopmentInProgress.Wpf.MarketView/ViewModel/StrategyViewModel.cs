using DevelopmentInProgress.Wpf.MarketView.Services;
using System;

namespace DevelopmentInProgress.Wpf.MarketView.ViewModel
{
    public class StrategyViewModel : BaseViewModel
    {
        public StrategyViewModel(IWpfExchangeService exchangeService)
            : base(exchangeService)
        {
        }

        public override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }
    }
}
