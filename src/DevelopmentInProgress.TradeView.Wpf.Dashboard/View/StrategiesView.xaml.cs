using DevelopmentInProgress.TradeView.Wpf.Dashboard.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.View;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.View
{
    /// <summary>
    /// Interaction logic for StrategiesView.xaml
    /// </summary>
    public partial class StrategiesView : DocumentViewBase
    {
        public StrategiesView(IViewContext viewContext, StrategiesViewModel strategiesViewModel)
            : base(viewContext, strategiesViewModel, Module.ModuleName)
        {
            InitializeComponent();

            DataContext = strategiesViewModel;
        }
    }
}
