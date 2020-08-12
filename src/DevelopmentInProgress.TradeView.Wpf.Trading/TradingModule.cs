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
    public class TradingModule : ModuleBase
    {
        public const string ModuleName = "Trading";
        private readonly static string AccountUser = $"Accounts";

        public TradingModule(ModuleNavigator moduleNavigator, ILoggerFacade logger)
            : base(moduleNavigator, logger)
        {
        }

        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<object, TradingView>(typeof(TradingView).Name);
            containerRegistry.Register<TradingPanelViewModel>(typeof(TradingPanelViewModel).Name);
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

            var userAccounts = await accountsService.GetAccountsAsync().ConfigureAwait(true);

            foreach (var userAccount in userAccounts.Accounts)
            {
                var accountDocument = CreateAccountModuleGroupItem(userAccount.AccountName, userAccount.AccountName);
                moduleGroup.ModuleGroupItems.Add(accountDocument);
            }

            moduleSettings.ModuleGroups.Add(moduleGroup);
            ModuleNavigator.AddModuleNavigation(moduleSettings);

            Logger.Log($"Initialized {this.GetType().FullName}", Category.Info, Priority.None);
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
