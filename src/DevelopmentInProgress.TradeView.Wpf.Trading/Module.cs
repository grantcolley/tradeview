using DevelopmentInProgress.TradeView.Wpf.Trading.View;
using DevelopmentInProgress.TradeView.Wpf.Trading.ViewModel;
using DevelopmentInProgress.TradeView.Wpf.Host.Module;
using DevelopmentInProgress.TradeView.Wpf.Host.Navigation;
using Microsoft.Practices.Unity;
using System;
using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using Prism.Logging;

namespace DevelopmentInProgress.TradeView.Wpf.Trading
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
            Container.RegisterType<object, TradingView>(typeof(TradingView).Name);
            Container.RegisterType<TradingViewModel>(typeof(TradingViewModel).Name);

            var moduleSettings = new ModuleSettings();
            moduleSettings.ModuleName = ModuleName;
            moduleSettings.ModuleImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Trading;component/Images/marketview.png";

            var moduleGroup = new ModuleGroup();
            moduleGroup.ModuleGroupName = AccountUser;

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

                Logger.Log("Initialize DevelopmentInProgress.TradeView.Wpf.Trading Complete", Category.Info, Priority.None);
            }
            catch(Exception ex)
            {
                Logger.Log($"Initialize DevelopmentInProgress.TradeView.Wpf.Trading failed to load: {ex.ToString()}", Category.Exception, Priority.None);
            }
        }

        private static ModuleGroupItem CreateAccountModuleGroupItem(string name, string title)
        {
            var accountDocument = new ModuleGroupItem();
            accountDocument.ModuleGroupItemName = name;
            accountDocument.TargetView = typeof(TradingView).Name;
            accountDocument.TargetViewTitle = title;
            accountDocument.ModuleGroupItemImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Trading;component/Images/account.png";
            return accountDocument;
        }
    }
}
