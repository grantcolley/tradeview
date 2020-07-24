using DevelopmentInProgress.TradeView.Wpf.Common.Services;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Module;
using DevelopmentInProgress.TradeView.Wpf.Host.Controller.Navigation;
using DevelopmentInProgress.TradeView.Wpf.Trading.View;
using DevelopmentInProgress.TradeView.Wpf.Trading.ViewModel;
using Prism.Ioc;
using Prism.Logging;
using System;

namespace DevelopmentInProgress.TradeView.Wpf.Trading
{
    public class Module : ModuleBase
    {
        public const string ModuleName = "Trading";
        private static string AccountUser = $"Accounts";

        public Module(ModuleNavigator moduleNavigator, ILoggerFacade logger)
            : base(moduleNavigator, logger)
        {
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<object, TradingView>(typeof(TradingView).Name);
            containerRegistry.Register<TradingViewModel>(typeof(TradingViewModel).Name);
        }

        public async override void OnInitialized(IContainerProvider containerProvider)
        {
            var moduleSettings = new ModuleSettings
            {
                ModuleName = ModuleName,
                ModuleImagePath = @"/DevelopmentInProgress.TradeView.Wpf.Trading;component/Images/marketview.png"
            };

            var moduleGroup = new ModuleGroup
            {
                ModuleGroupName = AccountUser
            };

            var accountsService = containerProvider.Resolve<IAccountsService>();

            try
            {
                var userAccounts = await accountsService.GetAccountsAsync().ConfigureAwait(true);

                foreach (var userAccount in userAccounts.Accounts)
                {
                    var accountDocument = CreateAccountModuleGroupItem(userAccount.AccountName, userAccount.AccountName);
                    moduleGroup.ModuleGroupItems.Add(accountDocument);
                }

                moduleSettings.ModuleGroups.Add(moduleGroup);
                ModuleNavigator.AddModuleNavigation(moduleSettings);

                Logger.Log("Initialized DevelopmentInProgress.TradeView.Wpf.Trading", Category.Info, Priority.None);
            }
            catch (Exception ex)
            {
                Logger.Log($"Initialize DevelopmentInProgress.TradeView.Wpf.Trading failed to load: {ex.ToString()}", Category.Exception, Priority.None);
            }
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
