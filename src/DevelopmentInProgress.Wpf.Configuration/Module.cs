using DevelopmentInProgress.Wpf.Host.Module;
using DevelopmentInProgress.Wpf.Host.Navigation;
using DevelopmentInProgress.Wpf.Configuration.View;
using DevelopmentInProgress.Wpf.Configuration.ViewModel;
using Microsoft.Practices.Unity;
using Prism.Logging;
using System;
using DevelopmentInProgress.Wpf.Host.View;
using DevelopmentInProgress.Wpf.Strategies.View;
using DevelopmentInProgress.Wpf.Trading.View;

namespace DevelopmentInProgress.Wpf.Configuration
{
    public class Module : ModuleBase
    {
        public const string ModuleName = "Configuration";
        private static string ConfigurationUser = $"Configuration : {Environment.UserName}";

        private const string StrategyModuleName = "Strategies";
        private static string StrategyUser = $"Strategies : {Environment.UserName}";

        private const string TradingModuleName = "Trading";
        private static string AccountUser = $"Accounts : {Environment.UserName}";

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
            Container.RegisterType<object, UserAccountsView>(typeof(UserAccountsView).Name);
            Container.RegisterType<UserAccountsViewModel>(typeof(UserAccountsViewModel).Name);

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

            var manageAccountsDocument = new ModuleGroupItem();
            manageAccountsDocument.ModuleGroupItemName = "Manage Accounts";
            manageAccountsDocument.TargetView = typeof(UserAccountsView).Name;
            manageAccountsDocument.TargetViewTitle = "Manage Accounts";
            manageAccountsDocument.ModuleGroupItemImagePath = @"/DevelopmentInProgress.Wpf.Configuration;component/Images/accounts.png";
            moduleGroup.ModuleGroupItems.Add(manageAccountsDocument);

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

        public static void AddAccount(string accountName)
        {
            var accountDocument = CreateAccountModuleGroupItem(accountName, accountName);

            var modulesNavigationView = StaticContainer.Resolve(typeof(ModulesNavigationView),
                typeof(ModulesNavigationView).Name) as ModulesNavigationView;

            modulesNavigationView.AddNavigationListItem(TradingModuleName, AccountUser, accountDocument);
        }

        public static void RemoveAccount(string accountName)
        {
            var modulesNavigationView = StaticContainer.Resolve(typeof(ModulesNavigationView),
                typeof(ModulesNavigationView).Name) as ModulesNavigationView;

            modulesNavigationView.RemoveNavigationListItem(TradingModuleName, AccountUser, accountName);
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

        private static ModuleGroupItem CreateAccountModuleGroupItem(string name, string title)
        {
            var accountDocument = new ModuleGroupItem();
            accountDocument.ModuleGroupItemName = name;
            accountDocument.TargetView = typeof(TradingView).Name;
            accountDocument.TargetViewTitle = title;
            accountDocument.ModuleGroupItemImagePath = @"/DevelopmentInProgress.Wpf.Trading;component/Images/account.png";
            return accountDocument;
        }
    }
}
