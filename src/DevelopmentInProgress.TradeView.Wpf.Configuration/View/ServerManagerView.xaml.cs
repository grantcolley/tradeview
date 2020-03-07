using DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.View;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.View
{
    /// <summary>
    /// Interaction logic for ServerManagerView.xaml
    /// </summary>
    public partial class ServerManagerView : DocumentViewBase
    {
        public ServerManagerView(IViewContext viewContext, ServerManagerViewModel serverManagerViewModel)
            : base(viewContext, serverManagerViewModel, Module.ModuleName)
        {
            InitializeComponent();

            DataContext = serverManagerViewModel;
        }
    }
}
