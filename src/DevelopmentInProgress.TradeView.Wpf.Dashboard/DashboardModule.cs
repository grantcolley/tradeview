using DevelopmentInProgress.TradeView.Wpf.Dashboard.View;
using DevelopmentInProgress.TradeView.Wpf.Dashboard.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Modular;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Navigation;
using Prism.Ioc;
using Prism.Logging;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard
{
    public class DashboardModule : ModuleBase
    {
        public const string ModuleName = "Dashboard";

        public DashboardModule(ModuleNavigator moduleNavigator, ILoggerFacade logger)
            : base(moduleNavigator, logger)
        {
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<object, ServerMonitorView>(typeof(ServerMonitorView).Name);
            containerRegistry.Register<ServerMonitorViewModel>(typeof(ServerMonitorViewModel).Name);
            containerRegistry.Register<object, AccountsView>(typeof(AccountsView).Name);
            containerRegistry.Register<AccountsViewModel>(typeof(AccountsViewModel).Name);
        }

        public override void OnInitialized(IContainerProvider containerProvider)
        {
            var moduleSettings = new ModuleSettings
            {
                ModuleName = ModuleName,
                ModuleImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Dashboard;component/Images/Dashboard.png"
            };

            var moduleGroup = new ModuleGroup
            {
                ModuleGroupName = "Dashboard"
            };

            var serverMonitor = new ModuleGroupItem
            {
                ModuleGroupItemName = "Servers",
                TargetView = typeof(ServerMonitorView).Name,
                TargetViewTitle = "Servers",
                ModuleGroupItemImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Dashboard;component/Images/ServerMonitor.png"
            };

            moduleGroup.ModuleGroupItems.Add(serverMonitor);

            var accountsMonitor = new ModuleGroupItem
            {
                ModuleGroupItemName = "Accounts",
                TargetView = typeof(AccountsView).Name,
                TargetViewTitle = "Accounts",
                ModuleGroupItemImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Dashboard;component/Images/ServerMonitor.png"
            };

            moduleGroup.ModuleGroupItems.Add(accountsMonitor);

            moduleSettings.ModuleGroups.Add(moduleGroup);
            ModuleNavigator.AddModuleNavigation(moduleSettings);

            Logger.Log($"Initialized {this.GetType().FullName}", Category.Info, Priority.None);
        }
    }
}
