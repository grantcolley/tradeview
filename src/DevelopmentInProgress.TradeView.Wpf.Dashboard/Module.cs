using DevelopmentInProgress.TradeView.Wpf.Dashboard.View;
using DevelopmentInProgress.TradeView.Wpf.Dashboard.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Host.Module;
using DevelopmentInProgress.TradeView.Wpf.Host.Navigation;
using Prism.Ioc;
using Prism.Logging;

namespace DevelopmentInProgress.TradeView.Wpf.Dashboard
{
    public class Module : ModuleBase
    {
        public const string ModuleName = "Dashboard";

        public Module(ModuleNavigator moduleNavigator, ILoggerFacade logger)
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
            var moduleSettings = new ModuleSettings();
            moduleSettings.ModuleName = ModuleName;
            moduleSettings.ModuleImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Dashboard;component/Images/Dashboard.png";

            var moduleGroup = new ModuleGroup();
            moduleGroup.ModuleGroupName = "Dashboard";

            var newDocument = new ModuleGroupItem();
            newDocument.ModuleGroupItemName = "Server Monitor";
            newDocument.TargetView = typeof(ServerMonitorView).Name;
            newDocument.TargetViewTitle = "Server Monitor";
            newDocument.ModuleGroupItemImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Dashboard;component/Images/ServerMonitor.png";

            moduleGroup.ModuleGroupItems.Add(newDocument);
            moduleSettings.ModuleGroups.Add(moduleGroup);
            ModuleNavigator.AddModuleNavigation(moduleSettings);

            Logger.Log("Initialize DevelopmentInProgress.TradeView.Wpf.Dashboard", Category.Info, Priority.None);
        }
    }
}
