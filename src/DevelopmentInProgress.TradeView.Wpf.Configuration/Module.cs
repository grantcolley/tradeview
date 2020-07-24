using DevelopmentInProgress.TradeView.Wpf.Configuration.View;
using DevelopmentInProgress.TradeView.Wpf.Configuration.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Module;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Navigation;
using DevelopmentInProgress.TradeView.Wpf.Strategies.View;
using DevelopmentInProgress.TradeView.Wpf.Trading.View;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.ViewModel;
using Prism.Ioc;
using Prism.Logging;

namespace DevelopmentInProgress.TradeView.Wpf.Configuration
{
    public class Module : ModuleBase
    {
        private static IContainerProvider staticContainerProvider;

        public const string ModuleName = "Configuration";
        private static string ConfigurationUser = $"Configuration";

        private const string StrategyModuleName = "Strategies";
        private static string StrategyUser = $"Strategies";

        private const string TradingModuleName = "Trading";
        private static string AccountUser = $"Accounts";

        public Module(ModuleNavigator moduleNavigator, ILoggerFacade logger)
            : base(moduleNavigator, logger)
        {
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<object, StrategyManagerView>(typeof(StrategyManagerView).Name);
            containerRegistry.Register<StrategyManagerViewModel>(typeof(StrategyManagerViewModel).Name);
            containerRegistry.Register<object, UserAccountsView>(typeof(UserAccountsView).Name);
            containerRegistry.Register<UserAccountsViewModel>(typeof(UserAccountsViewModel).Name);
            containerRegistry.Register<object, TradeServerManagerView>(typeof(TradeServerManagerView).Name);
            containerRegistry.Register<TradeServerManagerViewModel>(typeof(TradeServerManagerViewModel).Name);
        }

        public override void OnInitialized(IContainerProvider containerProvider)
        {
            staticContainerProvider = containerProvider;

            var moduleSettings = new ModuleSettings
            {
                ModuleName = ModuleName,
                ModuleImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Configuration;component/Images/configuration.png"
            };

            var moduleGroup = new ModuleGroup
            {
                ModuleGroupName = ConfigurationUser
            };

            var newDocument = new ModuleGroupItem
            {
                ModuleGroupItemName = "Manage Strategies",
                TargetView = typeof(StrategyManagerView).Name,
                TargetViewTitle = "Manage Strategies",
                ModuleGroupItemImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Configuration;component/Images/manageStrategies.png"
            };

            moduleGroup.ModuleGroupItems.Add(newDocument);

            var manageAccountsDocument = new ModuleGroupItem
            {
                ModuleGroupItemName = "Manage Accounts",
                TargetView = typeof(UserAccountsView).Name,
                TargetViewTitle = "Manage Accounts",
                ModuleGroupItemImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Configuration;component/Images/accounts.png"
            };

            moduleGroup.ModuleGroupItems.Add(manageAccountsDocument);

            var manageServersDocument = new ModuleGroupItem
            {
                ModuleGroupItemName = "Manage Trade Servers",
                TargetView = typeof(TradeServerManagerView).Name,
                TargetViewTitle = "Manage Servers",
                ModuleGroupItemImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Configuration;component/Images/manageServers.png"
            };

            moduleGroup.ModuleGroupItems.Add(manageServersDocument);

            moduleSettings.ModuleGroups.Add(moduleGroup);
            ModuleNavigator.AddModuleNavigation(moduleSettings);

            Logger.Log("Initialized DevelopmentInProgress.TradeView.Wpf.Configuration", Category.Info, Priority.None);
        }

        public static void AddStrategy(string strategyName)
        {
            var strategyDocument = CreateStrategyModuleGroupItem(strategyName, strategyName);

            var moduleNavigator = staticContainerProvider.Resolve<ModuleNavigator>();

            moduleNavigator.AddNavigationListItem(StrategyModuleName, StrategyUser, strategyDocument);
        }

        public static void RemoveStrategy(string strategyName)
        {
            var moduleNavigator = staticContainerProvider.Resolve<ModuleNavigator>();

            moduleNavigator.RemoveNavigationListItem(StrategyModuleName, StrategyUser, strategyName);
        }

        public static void AddAccount(string accountName)
        {
            var accountDocument = CreateAccountModuleGroupItem(accountName, accountName);

            var moduleNavigator = staticContainerProvider.Resolve<ModuleNavigator>();

            moduleNavigator.AddNavigationListItem(TradingModuleName, AccountUser, accountDocument);
        }

        public static void RemoveAccount(string accountName)
        {
            var moduleNavigator = staticContainerProvider.Resolve<ModuleNavigator>();

            moduleNavigator.RemoveNavigationListItem(TradingModuleName, AccountUser, accountName);
        }

        private static ModuleGroupItem CreateStrategyModuleGroupItem(string name, string title)
        {
            var strategyDocument = new ModuleGroupItem
            {
                ModuleGroupItemName = name,
                TargetView = typeof(StrategyRunnerView).Name,
                TargetViewTitle = title,
                ModuleGroupItemImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Strategies;component/Images/strategy.png"
            };

            return strategyDocument;
        }

        private static ModuleGroupItem CreateAccountModuleGroupItem(string name, string title)
        {
            var accountDocument = new ModuleGroupItem
            {
                ModuleGroupItemName = name,
                TargetView = typeof(TradingView).Name,
                TargetViewTitle = title,
                ModuleGroupItemImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Trading;component/Images/account.png"
            };

            return accountDocument;
        }
    }
}
