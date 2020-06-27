using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.View;
using DevelopmentInProgress.TradeView.Wpf.Trading.ViewModel;

namespace DevelopmentInProgress.TradeView.Wpf.Trading.View
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
