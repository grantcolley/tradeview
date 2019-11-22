using DevelopmentInProgress.TradeView.Wpf.Host.Context;
using DevelopmentInProgress.TradeView.Wpf.Host.View;
using DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration.View
{
    /// <summary>
    /// Interaction logic for UserAccountsView.xaml
    /// </summary>
    public partial class UserAccountsView : DocumentViewBase
    {
        public UserAccountsView(IViewContext viewContext, UserAccountsViewModel userAccountsViewModel)
            : base(viewContext, userAccountsViewModel, Module.ModuleName)
        {
            InitializeComponent();

            DataContext = userAccountsViewModel;
        }
    }
}
