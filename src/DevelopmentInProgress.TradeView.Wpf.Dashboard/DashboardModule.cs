using DevelopmentInProgress.TradeView.Wpf.Dashboard.View;
using DevelopmentInProgress.TradeView.Wpf.Dashboard.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Module;
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

            var newDocument = new ModuleGroupItem
            {
                ModuleGroupItemName = "Server Monitor",
                TargetView = typeof(ServerMonitorView).Name,
                TargetViewTitle = "Server Monitor",
                ModuleGroupItemImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Dashboard;component/Images/ServerMonitor.png"
            };

            moduleGroup.ModuleGroupItems.Add(newDocument);
            moduleSettings.ModuleGroups.Add(moduleGroup);
            ModuleNavigator.AddModuleNavigation(moduleSettings);

            Logger.Log($"Initialized {this.GetType().FullName}", Category.Info, Priority.None);
        }
    }
}
