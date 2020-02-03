using DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.View;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.View
{
    /// <summary>
    /// Interaction logic for StrategyServerManagerView.xaml
    /// </summary>
    public partial class StrategyServerManagerView : DocumentViewBase
    {
        public StrategyServerManagerView(IViewContext viewContext, StrategyServerManagerViewModel strategyServerManagerViewModel)
            : base(viewContext, strategyServerManagerViewModel, Module.ModuleName)
        {
            InitializeComponent();

            DataContext = strategyServerManagerViewModel;
        }
    }
}
