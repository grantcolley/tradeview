using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.MarketView.ViewModel;
using DevelopmentInProgress.Wpf.Host.View;

namespace DevelopmentInProgress.Wpf.MarketView.View
{
    /// <summary>
    /// Interaction logic for TradingView.xaml
    /// </summary>
    public partial class TradingView : DocumentViewBase
    {
        public TradingView(IViewContext viewContext, TradingViewModel tradingViewModel)
            : base(viewContext, tradingViewModel, Module.ModuleName)
        {
            InitializeComponent();

            DataContext = tradingViewModel;
        }
    }
}
