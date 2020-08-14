using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.View;
using DevelopmentInProgress.TradeView.Wpf.Strategies.Events;
using DevelopmentInProgress.TradeView.Wpf.Strategies.ViewModel;
using System;

namespace DevelopmentInProgress.TradeView.Wpf.Strategies.View
{
    /// <summary>
    /// Interaction logic for StrategyView.xaml
    /// </summary>
    public partial class StrategyRunnerView : DocumentViewBase
    {
        public StrategyRunnerView(IViewContext viewContext, StrategyRunnerViewModel strategyRunnerViewModel)
            : base(viewContext, strategyRunnerViewModel, StrategiesModule.ModuleName)
        {
            InitializeComponent();

            if(strategyRunnerViewModel == null)
            {
                throw new ArgumentNullException(nameof(strategyRunnerViewModel));
            }

            strategyRunnerViewModel.OnStrategyDisplay += OnStrategyDisplay;

            DataContext = strategyRunnerViewModel;
        }

        private void OnStrategyDisplay(object sender, StrategyDisplayEventArgs e)
        {
            StrategyDisplayContent.Content = e.StrategyAssemblyManager.StrategyDisplayView;
        }
    }
}
