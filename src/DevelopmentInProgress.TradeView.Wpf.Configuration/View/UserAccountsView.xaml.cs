using DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.View;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.View
{
    /// <summary>
    /// Interaction logic for UserAccountsView.xaml
    /// </summary>
    public partial class UserAccountsView : DocumentViewBase
    {
        public UserAccountsView(IViewContext viewContext, UserAccountsViewModel userAccountsViewModel)
            : base(viewContext, userAccountsViewModel, ConfigurationModule.ModuleName)
        {
            InitializeComponent();

            DataContext = userAccountsViewModel;
        }
    }
}
