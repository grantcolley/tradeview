using DevelopmentInProgress.Wpf.Host.Context;
using DevelopmentInProgress.Wpf.Host.View;
using DevelopmentInProgress.Wpf.Configuration.ViewModel;

namespace DevelopmentInProgress.Wpf.Configuration.View
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
