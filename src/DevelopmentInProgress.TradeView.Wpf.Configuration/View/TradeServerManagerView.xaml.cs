using DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.View;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.View
{
    /// <summary>
    /// Interaction logic for ServerManagerView.xaml
    /// </summary>
    public partial class TradeServerManagerView : DocumentViewBase
    {
        public TradeServerManagerView(IViewContext viewContext, TradeServerManagerViewModel serverManagerViewModel)
            : base(viewContext, serverManagerViewModel, Module.ModuleName)
        {
            InitializeComponent();

            DataContext = serverManagerViewModel;
        }
    }
}
