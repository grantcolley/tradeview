using DevelopmentInProgress.TradeView.Wpf.Dashboard.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.View;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard.View
{
    /// <summary>
    /// Interaction logic for AccountsView.xaml
    /// </summary>
    public partial class AccountsView : DocumentViewBase
    {
        public AccountsView(IViewContext viewContext, AccountsViewModel accountsViewModel)
            : base(viewContext, accountsViewModel, DashboardModule.ModuleName)
        {
            InitializeComponent();

            DataContext = accountsViewModel;
        }
    }
}
