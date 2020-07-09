using DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.View;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.View
{
    /// <summary>
    /// Interaction logic for StrategyManagerView.xaml
    /// </summary>
    public partial class StrategyManagerView : DocumentViewBase
    {
        public StrategyManagerView(IViewContext viewContext, StrategyManagerViewModel strategyManagerViewModel)
            : base(viewContext, strategyManagerViewModel, Module.ModuleName)
        {
            InitializeComponent();

            DataContext = strategyManagerViewModel;
        }
    }
}
