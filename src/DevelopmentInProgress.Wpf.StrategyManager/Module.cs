using DevelopmentInProgress.Wpf.Host.Module;
using DevelopmentInProgress.Wpf.Host.Navigation;
using DevelopmentInProgress.Wpf.StrategyManager.View;
using DevelopmentInProgress.Wpf.StrategyManager.ViewModel;
using Microsoft.Practices.Unity;
using Prism.Logging;

namespace DevelopmentInProgress.Wpf.StrategyManager
{
    public class Module : ModuleBase
    {
        public const string ModuleName = "Strategy Manager";

        public Module(IUnityContainer container, ModuleNavigator moduleNavigator, ILoggerFacade logger)
            : base(container, moduleNavigator, logger)
        {
        }

        public override void Initialize()
        {
            Container.RegisterType<object, StrategyManagerView>(typeof(StrategyManagerView).Name);
            Container.RegisterType<StrategyManagerViewModel>(typeof(StrategyManagerViewModel).Name);

            var moduleSettings = new ModuleSettings();
            moduleSettings.ModuleName = ModuleName;
            moduleSettings.ModuleImagePath = @"/DevelopmentInProgress.Wpf.StrategyManager;component/Images/strategyManager.png";

            var moduleGroup = new ModuleGroup();
            moduleGroup.ModuleGroupName = "Strategy Manager";

            var newDocument = new ModuleGroupItem();
            newDocument.ModuleGroupItemName = "Manage Strategies";
            newDocument.TargetView = typeof(StrategyManagerView).Name;
            newDocument.TargetViewTitle = "Manage Strategies";
            newDocument.ModuleGroupItemImagePath = @"/DevelopmentInProgress.Wpf.StrategyManager;component/Images/manageStrategies.png";

            moduleGroup.ModuleGroupItems.Add(newDocument);
            moduleSettings.ModuleGroups.Add(moduleGroup);
            ModuleNavigator.AddModuleNavigation(moduleSettings);

            Logger.Log("Initialize DevelopmentInProgress.Wpf.StrategyManager Complete", Category.Info, Priority.None);
        }
    }
}
