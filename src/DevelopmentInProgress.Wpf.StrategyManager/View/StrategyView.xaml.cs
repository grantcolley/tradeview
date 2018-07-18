using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.View;
using DevelopmentInProgress.Wpf.StrategyManager.ViewModel;

namespace DevelopmentInProgress.Wpf.StrategyManager.View
{
    /// <summary>
    /// Interaction logic for StrategyView.xaml
    /// </summary>
    public partial class StrategyView : DocumentViewBase
    {
        public StrategyView(IViewContext viewContext, StrategyViewModel strategyViewModel)
            : base(viewContext, strategyViewModel, Module.ModuleName)
        {
            InitializeComponent();

            DataContext = strategyViewModel;
        }
    }
}
