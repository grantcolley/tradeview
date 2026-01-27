using DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.View;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.View
{
    /// <summary>
    /// Interaction logic for TradeServerManagerView.xaml
    /// </summary>
    public partial class TradeServerManagerView : DocumentViewBase
    {
        public TradeServerManagerView(IViewContext viewContext, TradeServerManagerViewModel tradeServerManagerViewModel)
            : base(viewContext, tradeServerManagerViewModel, ConfigurationModule.ModuleName)
        {
            InitializeComponent();

            DataContext = tradeServerManagerViewModel;
        }
    }
}
