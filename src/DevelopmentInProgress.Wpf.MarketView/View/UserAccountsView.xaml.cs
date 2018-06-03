using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.View;
using DevelopmentInProgress.Wpf.MarketView.ViewModel;

namespace DevelopmentInProgress.Wpf.MarketView.View
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
