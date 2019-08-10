using DevelopmentInProgress.Wpf.Trading.View;
using DevelopmentInProgress.Wpf.Trading.ViewModel;
using DevelopmentInProgress.Wpf.Host.Module;
using DevelopmentInProgress.Wpf.Host.Navigation;
using Microsoft.Practices.Unity;
using System;
using DevelopmentInProgress.Wpf.Trading.Services;
using DevelopmentInProgress.Wpf.Host.View;
using Prism.Logging;

namespace DevelopmentInProgress.Wpf.Trading
{
    public class Module : ModuleBase
    {
        public const string ModuleName = "Trading";
        private static string AccountUser = $"Accounts : {Environment.UserName}";
        private static IUnityContainer StaticContainer;

        public Module(IUnityContainer container, ModuleNavigator moduleNavigator, ILoggerFacade logger)
            : base(container, moduleNavigator, logger)
        {
            StaticContainer = Container;
        }

        public async override void Initialize()
        {
            Container.RegisterType<object, UserAccountsView>(typeof(UserAccountsView).Name);
            Container.RegisterType<UserAccountsViewModel>(typeof(UserAccountsViewModel).Name);
            Container.RegisterType<object, TradingView>(typeof(TradingView).Name);
            Container.RegisterType<TradingViewModel>(typeof(TradingViewModel).Name);

            var moduleSettings = new ModuleSettings();
            moduleSettings.ModuleName = ModuleName;
            moduleSettings.ModuleImagePath = @"/DevelopmentInProgress.Wpf.Trading;component/Images/marketview.png";

            var moduleGroup = new ModuleGroup();
            moduleGroup.ModuleGroupName = AccountUser;

            var manageAccountsDocument = new ModuleGroupItem();
            manageAccountsDocument.ModuleGroupItemName = "Manage Accounts";
            manageAccountsDocument.TargetView = typeof(UserAccountsView).Name;
            manageAccountsDocument.TargetViewTitle = "Manage Accounts";
            manageAccountsDocument.ModuleGroupItemImagePath = @"/DevelopmentInProgress.Wpf.Trading;component/Images/accounts.png";
            moduleGroup.ModuleGroupItems.Add(manageAccountsDocument);

            var accountsService = Container.Resolve<IAccountsService>();

            try
            {
                var userAccounts = await accountsService.GetAccounts();

                foreach (var userAccount in userAccounts.Accounts)
                {
                    var accountDocument = CreateAccountModuleGroupItem(userAccount.AccountName, userAccount.AccountName);
                    moduleGroup.ModuleGroupItems.Add(accountDocument);
                }

                moduleSettings.ModuleGroups.Add(moduleGroup);
                ModuleNavigator.AddModuleNavigation(moduleSettings);

                Logger.Log("Initialize DevelopmentInProgress.Wpf.Trading Complete", Category.Info, Priority.None);
            }
            catch(Exception ex)
            {
                Logger.Log($"Initialize DevelopmentInProgress.Wpf.Trading failed to load: {ex.ToString()}", Category.Exception, Priority.None);
            }
        }

        public static void AddAccount(string accountName)
        {
            var accountDocument = CreateAccountModuleGroupItem(accountName, accountName);

            var modulesNavigationView = StaticContainer.Resolve(typeof(ModulesNavigationView),
                typeof(ModulesNavigationView).Name) as ModulesNavigationView;

            modulesNavigationView.AddNavigationListItem(ModuleName, AccountUser, accountDocument);
        }

        public static void RemoveAccount(string accountName)
        {
            var modulesNavigationView = StaticContainer.Resolve(typeof(ModulesNavigationView),
                typeof(ModulesNavigationView).Name) as ModulesNavigationView;

            modulesNavigationView.RemoveNavigationListItem(ModuleName, AccountUser, accountName);
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
