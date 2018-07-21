using DevelopmentInProgress.Wpf.Host.Module;
using DevelopmentInProgress.Wpf.Host.Navigation;
using DevelopmentInProgress.Wpf.Host.View;
using DevelopmentInProgress.Wpf.StrategyManager.Services;
using DevelopmentInProgress.Wpf.StrategyManager.View;
using DevelopmentInProgress.Wpf.StrategyManager.ViewModel;
using Microsoft.Practices.Unity;
using Prism.Logging;

namespace DevelopmentInProgress.Wpf.StrategyManager
{
    public class Module : ModuleBase
    {
        public const string ModuleName = "Strategy Manager";
        private static IUnityContainer StaticContainer;

        public Module(IUnityContainer container, ModuleNavigator moduleNavigator, ILoggerFacade logger)
            : base(container, moduleNavigator, logger)
        {
            StaticContainer = container;
        }

        public override void Initialize()
        {
            Container.RegisterType<IStrategyService, StrategyService>();
            Container.RegisterType<object, StrategyManagerView>(typeof(StrategyManagerView).Name);
            Container.RegisterType<StrategyManagerViewModel>(typeof(StrategyManagerViewModel).Name);
            Container.RegisterType<object, StrategyView>(typeof(StrategyView).Name);
            Container.RegisterType<StrategyViewModel>(typeof(StrategyViewModel).Name);

            var moduleSettings = new ModuleSettings();
            moduleSettings.ModuleName = ModuleName;
            moduleSettings.ModuleImagePath = @"/DevelopmentInProgress.Wpf.StrategyManager;component/Images/strategyManager.png";

            var moduleGroup = new ModuleGroup();
            moduleGroup.ModuleGroupName = ModuleName;

            var newDocument = new ModuleGroupItem();
            newDocument.ModuleGroupItemName = "Manage Strategies";
            newDocument.TargetView = typeof(StrategyManagerView).Name;
            newDocument.TargetViewTitle = "Manage Strategies";
            newDocument.ModuleGroupItemImagePath = @"/DevelopmentInProgress.Wpf.StrategyManager;component/Images/manageStrategies.png";
            moduleGroup.ModuleGroupItems.Add(newDocument);

            var strategyService = Container.Resolve<IStrategyService>();

            var userStrategies = strategyService.GetStrategies();

            foreach (var strategy in userStrategies)
            {
                var strategyDocument = CreateStrategyModuleGroupItem(strategy.Name, strategy.Name);
                moduleGroup.ModuleGroupItems.Add(strategyDocument);
            }

            moduleSettings.ModuleGroups.Add(moduleGroup);
            ModuleNavigator.AddModuleNavigation(moduleSettings);

            Logger.Log("Initialize DevelopmentInProgress.Wpf.StrategyManager Complete", Category.Info, Priority.None);
        }

        public static void AddStrategy(string strategyName)
        {
            var strategyDocument = CreateStrategyModuleGroupItem(strategyName, strategyName);

            var modulesNavigationView = StaticContainer.Resolve(typeof(ModulesNavigationView),
                typeof(ModulesNavigationView).Name) as ModulesNavigationView;

            modulesNavigationView.AddNavigationListItem(ModuleName, ModuleName, strategyDocument);
        }

        public static void RemoveStrategy(string strategyName)
        {
            var modulesNavigationView = StaticContainer.Resolve(typeof(ModulesNavigationView),
                typeof(ModulesNavigationView).Name) as ModulesNavigationView;

            modulesNavigationView.RemoveNavigationListItem(ModuleName, ModuleName, strategyName);
        }

        private static ModuleGroupItem CreateStrategyModuleGroupItem(string name, string title)
        {
            var strategyDocument = new ModuleGroupItem();
            strategyDocument.ModuleGroupItemName = name;
            strategyDocument.TargetView = typeof(StrategyView).Name;
            strategyDocument.TargetViewTitle = title;
            strategyDocument.ModuleGroupItemImagePath = @"/DevelopmentInProgress.Wpf.StrategyManager;component/Images/strategy.png";
            return strategyDocument;
        }
    }
}
