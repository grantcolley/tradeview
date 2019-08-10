using DevelopmentInProgress.Wpf.Host.Module;
using DevelopmentInProgress.Wpf.Host.Navigation;
using DevelopmentInProgress.Wpf.Configuration.View;
using DevelopmentInProgress.Wpf.Configuration.ViewModel;
using Microsoft.Practices.Unity;
using Prism.Logging;
using System;
using DevelopmentInProgress.Wpf.Host.View;
using DevelopmentInProgress.Wpf.Strategies.View;

namespace DevelopmentInProgress.Wpf.Configuration
{
    public class Module : ModuleBase
    {
        public const string ModuleName = "Configuration";
        private static string ConfigurationUser = $"Configuration : {Environment.UserName}";

        public const string StrategyModuleName = "Strategies";
        private static string StrategyUser = $"Strategies : {Environment.UserName}";

        private static IUnityContainer StaticContainer;

        public Module(IUnityContainer container, ModuleNavigator moduleNavigator, ILoggerFacade logger)
            : base(container, moduleNavigator, logger)
        {
            StaticContainer = container;
        }

        public override void Initialize()
        {
            Container.RegisterType<object, StrategyManagerView>(typeof(StrategyManagerView).Name);
            Container.RegisterType<StrategyManagerViewModel>(typeof(StrategyManagerViewModel).Name);

            var moduleSettings = new ModuleSettings();
            moduleSettings.ModuleName = ModuleName;
            moduleSettings.ModuleImagePath = @"/DevelopmentInProgress.Wpf.Configuration;component/Images/configuration.png";

            var moduleGroup = new ModuleGroup();
            moduleGroup.ModuleGroupName = ConfigurationUser;

            var newDocument = new ModuleGroupItem();
            newDocument.ModuleGroupItemName = "Manage Strategies";
            newDocument.TargetView = typeof(StrategyManagerView).Name;
            newDocument.TargetViewTitle = "Manage Strategies";
            newDocument.ModuleGroupItemImagePath = @"/DevelopmentInProgress.Wpf.Configuration;component/Images/manageStrategies.png";
            moduleGroup.ModuleGroupItems.Add(newDocument);

            moduleSettings.ModuleGroups.Add(moduleGroup);
            ModuleNavigator.AddModuleNavigation(moduleSettings);

            Logger.Log("Initialize DevelopmentInProgress.Wpf.Configuration Complete", Category.Info, Priority.None);
        }

        public static void AddStrategy(string strategyName)
        {
            var strategyDocument = CreateStrategyModuleGroupItem(strategyName, strategyName);

            var modulesNavigationView = StaticContainer.Resolve(typeof(ModulesNavigationView),
                typeof(ModulesNavigationView).Name) as ModulesNavigationView;

            modulesNavigationView.AddNavigationListItem(StrategyModuleName, StrategyUser, strategyDocument);
        }

        public static void RemoveStrategy(string strategyName)
        {
            var modulesNavigationView = StaticContainer.Resolve(typeof(ModulesNavigationView),
                typeof(ModulesNavigationView).Name) as ModulesNavigationView;

            modulesNavigationView.RemoveNavigationListItem(StrategyModuleName, StrategyUser, strategyName);
        }

        private static ModuleGroupItem CreateStrategyModuleGroupItem(string name, string title)
        {
            var strategyDocument = new ModuleGroupItem();
            strategyDocument.ModuleGroupItemName = name;
            strategyDocument.TargetView = typeof(StrategyRunnerView).Name;
            strategyDocument.TargetViewTitle = title;
            strategyDocument.ModuleGroupItemImagePath = @"/DevelopmentInProgress.Wpf.Strategies;component/Images/strategy.png";
            return strategyDocument;
        }
    }
}
