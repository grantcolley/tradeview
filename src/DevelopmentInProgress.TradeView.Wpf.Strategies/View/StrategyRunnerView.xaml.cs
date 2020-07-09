using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.View;
using DevelopmentInProgress.TradeView.Wpf.Strategies.Events;
using DevelopmentInProgress.TradeView.Wpf.Strategies.ViewModel;

namespace DevelopmentInProgress.TradeView.Wpf.Strategies.View
{
    /// <summary>
    /// Interaction logic for StrategyView.xaml
    /// </summary>
    public partial class StrategyRunnerView : DocumentViewBase
    {
        public StrategyRunnerView(IViewContext viewContext, StrategyRunnerViewModel strategyRunnerViewModel)
            : base(viewContext, strategyRunnerViewModel, Module.ModuleName)
        {
            InitializeComponent();

            strategyRunnerViewModel.OnStrategyDisplay += OnStrategyDisplay;

            DataContext = strategyRunnerViewModel;
        }

        private void OnStrategyDisplay(object sender, StrategyDisplayEventArgs e)
        {
            StrategyDisplayContent.Content = e.StrategyAssemblyManager.StrategyDisplayView;
        }
    }
}
