using DevelopmentInProgress.TradeView.Wpf.Dashboard.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.View;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.View
{
    /// <summary>
    /// Interaction logic for ServerMonitorView.xaml
    /// </summary>
    public partial class ServerMonitorView : DocumentViewBase
    {
        public ServerMonitorView(IViewContext viewContext, ServerMonitorViewModel serverMonitorViewModel)
            : base(viewContext, serverMonitorViewModel, Module.ModuleName)
        {
            InitializeComponent();

            DataContext = serverMonitorViewModel;
        }
    }
}
