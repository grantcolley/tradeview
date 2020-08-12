using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.View;
using DevelopmentInProgress.TradeView.Wpf.Trading.ViewModel;

namespace DevelopmentInProgress.TradeView.Wpf.Trading.View
{
    /// <summary>
    /// Interaction logic for TradingView.xaml
    /// </summary>
    public partial class TradingView : DocumentViewBase
    {
        public TradingView(IViewContext viewContext, TradingViewModel tradingViewModel)
            : base(viewContext, tradingViewModel, TradingModule.ModuleName)
        {
            InitializeComponent();

            DataContext = tradingViewModel;
        }
    }
}
