using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.View;
using DevelopmentInProgress.Wpf.StrategyManager.ViewModel;

namespace DevelopmentInProgress.Wpf.StrategyManager.View
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
